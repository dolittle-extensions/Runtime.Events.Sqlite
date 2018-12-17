using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Sqlite.Persistence;
using Dolittle.Runtime.Events.Store;
using Dolittle.Serialization.Json;
using Microsoft.EntityFrameworkCore;

namespace Dolittle.Runtime.Events.Sqlite.Store
{
    /// <summary>
    /// A Sqlite implementation of <see cref="Dolittle.Runtime.Events.Store.IEventStore" />
    /// </summary>
    [SingletonPerTenant]
    public class EventStore : IEventStore
    {
        private readonly Database _database;
        private readonly ILogger _logger;
        private readonly ISerializer _serializer;

        /// <summary>
        /// Instantiates an instance of <see cref="EventStore"/>
        /// </summary>
        /// <param name="database">Database configuration</param>
        /// <param name="serializer"></param>
        /// <param name="logger">An instance of <see cref="ILogger" /></param>
        public EventStore(Database database, ISerializer serializer, ILogger logger)
        {
            _database = database;
            _logger = logger;
            _serializer = serializer;
        }

        /// <inheritdoc />
        public CommittedEventStream Commit(UncommittedEventStream uncommittedEvents)
        {
            var commit = Persistence.Commit.From(uncommittedEvents, _serializer);
            using (var es = _database.GetContext())
            {
                es.Commits.Add(commit);
                es.SaveChanges();
                return commit.ToCommittedEventStream(_serializer);
            }
        }

        /// <inheritdoc />
        public Commits Fetch(EventSourceKey eventSourceKey)
        {
            using (var es = _database.GetContext())
            {
                return BuildCommits(es.Commits.Include(c => c.Events).Where(c => c.EventSourceId == eventSourceKey.Id.Value).OrderBy(c => c.Id));
            }
        }

        /// <inheritdoc />
        public Commits FetchAllCommitsAfter(CommitSequenceNumber commit)
        {
            using (var es = _database.GetContext())
            {
                return BuildCommits(es.Commits.Include(c => c.Events).Where(c => c.Id > (long)commit.Value).OrderBy(c => c.Id));
            }
        }


        const string GET_EVENTS_BY_TYPE = "SELECT * FROM Events e WHERE e.EventArtifact=@artifact ORDER BY e.CommitId ASC";
        /// <inheritdoc />
        public SingleEventTypeEventStream FetchAllEventsOfType(ArtifactId eventType)
        {
            using (var es = _database.GetContext())
            {
                return GetStreamFromEvents(es.Events.Where(e => e.EventArtifact == eventType).OrderBy(e => e.CommitId));
            }
        }

        /// <inheritdoc />
        public SingleEventTypeEventStream FetchAllEventsOfTypeAfter(ArtifactId eventType, CommitSequenceNumber commit)
        {
            using (var es = _database.GetContext())
            {
                return GetStreamFromEvents(es.Events.Where(e => e.EventArtifact == eventType && e.CommitId > (long)commit.Value).OrderBy(e => e.CommitId));
            }
        }

        /// <inheritdoc />
        public Commits FetchFrom(EventSourceKey eventSourceKey, CommitVersion commitVersion)
        {
            using (var es = _database.GetContext())
            {
                return BuildCommits(es.Commits.Include(c => c.Events).Where(c => c.Id >= (long)commitVersion.Value && c.EventSourceId == eventSourceKey.Id.Value).OrderBy(c => c.Id));
            }
        }

        const string GET_CURRENT_VERSION = "SELECT c.CommitNumber, c.Sequence FROM Commits c where c.EventSourceId=@eventSource and c.EventSourceArtifact = @artifact order by c.CommitNumber desc LIMIT 1";

        /// <inheritdoc />
        public EventSourceVersion GetCurrentVersionFor(EventSourceKey eventSource)
        {
            using (var es = _database.GetContext())
            {
                var result = es.FromSql(GET_CURRENT_VERSION,new Dictionary<string,object>(){
                    { "@eventSource",eventSource.Id.Value },
                    { "@artifact",eventSource.Artifact.Value }
                }).FirstOrDefault();
                if(result != null)
                {
                    return new EventSourceVersion((ulong)result.CommitNumber,(uint)result.Sequence);
                } 
                else 
                {
                    return EventSourceVersion.NoVersion;
                }
            }
        }

        /// <inheritdoc />
        public EventSourceVersion GetNextVersionFor(EventSourceKey eventSource)
        {
            return GetCurrentVersionFor(eventSource).NextCommit();
        }

        #region IDisposable Support
        /// <summary>
        /// Detects redundant calls to Dispose
        /// </summary>
        protected bool disposedValue = false;

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~EventStore() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        /// <inheritdoc />
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        Persistence.Commit ToCommit(UncommittedEventStream uncommittedEvents)
        {
            var events = uncommittedEvents.Events.Select(e => 
                new Persistence.Event 
                {
                    Id = e.Id.Value,
                    CorrelationId = e.Metadata.CorrelationId.Value,
                    EventArtifact =  e.Metadata.Artifact.Id.Value,
                    Generation =  e.Metadata.Artifact.Generation.Value,
                    EventSourceArtifact =  uncommittedEvents.Source.Artifact.Value,
                    EventSourceId =  e.Metadata.EventSourceId.Value,
                    Commit =  e.Metadata.VersionedEventSource.Version.Commit,
                    Sequence =  e.Metadata.VersionedEventSource.Version.Sequence,
                    Occurred =  e.Metadata.Occurred.ToUnixTimeMilliseconds(),
                    OriginalContext =  _serializer.ToJson(Persistence.OriginalContext.From(e.Metadata.OriginalContext)),
                    EventData = PropertyBagSerializer.Serialize(e.Event,_serializer)
                }
            ).ToList();

            var commit = new Commit
            {
                Id = 0 ,
                CorrelationId = uncommittedEvents.CorrelationId.Value,
                CommitId = uncommittedEvents.Id.Value,
                Timestamp = uncommittedEvents.Timestamp.ToUnixTimeMilliseconds(),
                EventSourceId = uncommittedEvents.Source.EventSource.Value,
                EventSourceArtifact = uncommittedEvents.Source.Artifact.Value,
                CommitNumber = uncommittedEvents.Source.Version.Commit,
                Sequence = uncommittedEvents.Source.Version.Sequence,
                Events = events 
            };
  
            return commit;
        }

        SingleEventTypeEventStream GetStreamFromEvents(IEnumerable<Persistence.Event> events)
        {
            return new SingleEventTypeEventStream(events.Select(e => new CommittedEventEnvelope((ulong)e.CommitId,e.ToEventMetadata(_serializer),e.ToPropertyBag(_serializer))));
        }

        Commits BuildCommits(IEnumerable<Persistence.Commit> commits)
        {
            return new Commits(commits.Select(c => c.ToCommittedEventStream(_serializer)).ToList());
        }        
    }
}