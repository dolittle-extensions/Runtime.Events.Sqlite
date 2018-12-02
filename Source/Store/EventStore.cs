using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.SqlLite.Store
{
    /// <summary>
    /// A SqlLite implementation of <see cref="Dolittle.Runtime.Events.Store.IEventStore" />
    /// </summary>
    public class EventStore : IEventStore
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Instantiates an instance of <see cref="EventStore"/>
        /// </summary>
        /// <param name="logger">An instance of <see cref="ILogger" /></param>
        public EventStore(ILogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public CommittedEventStream Commit(UncommittedEventStream uncommittedEvents)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Commits Fetch(EventSourceKey eventSourceKey)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Commits FetchAllCommitsAfter(CommitSequenceNumber commit)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public SingleEventTypeEventStream FetchAllEventsOfType(ArtifactId eventType)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public SingleEventTypeEventStream FetchAllEventsOfTypeAfter(ArtifactId eventType, CommitSequenceNumber commit)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Commits FetchFrom(EventSourceKey eventSourceKey, CommitVersion commitVersion)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public EventSourceVersion GetCurrentVersionFor(EventSourceKey eventSource)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public EventSourceVersion GetNextVersionFor(EventSourceKey eventSource)
        {
            throw new System.NotImplementedException();
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

    }
}