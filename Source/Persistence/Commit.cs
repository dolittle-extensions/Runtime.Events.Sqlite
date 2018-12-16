using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Events.Store;
using Dolittle.Serialization.Json;
using Newtonsoft.Json;

namespace Dolittle.Runtime.Events.Sqlite.Persistence
{
    /// <summary>
    /// 
    /// </summary>
    public class Commit
    {
        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }  
        /// <summary>
        /// 
        /// </summary>
        public Guid CorrelationId { get; set; }  
        /// <summary>
        /// 
        /// </summary>
        public Guid CommitId { get; set; }     
        /// <summary>
        /// 
        /// </summary>
        public long Timestamp { get; set; }    
        /// <summary>
        /// 
        /// </summary>
        public Guid EventSourceId { get; set; }    
        /// <summary>
        /// 
        /// </summary>
        public Guid EventSourceArtifact { get; set; } 
        /// <summary>
        /// 
        /// </summary>
        public ulong CommitNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public uint Sequence { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<Persistence.Event> Events { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventStream"></param>
        /// <param name="jsonSerializer"></param>
        /// <returns></returns>
        public static Commit From(UncommittedEventStream eventStream, ISerializer jsonSerializer)
        {
            var events = eventStream.Events.Select(e => new Event
            {
                Id = e.Id,
                CorrelationId = e.Metadata.CorrelationId,
                EventArtifact = e.Metadata.Artifact.Id,
                EventSourceId = e.Metadata.EventSourceId,
                Generation = e.Metadata.Artifact.Generation,
                EventSourceArtifact = eventStream.Source.Artifact,
                Commit = e.Metadata.VersionedEventSource.Version.Commit,
                Sequence = e.Metadata.VersionedEventSource.Version.Sequence,
                Occurred =  e.Metadata.Occurred.ToUnixTimeMilliseconds(),
                OriginalContext = jsonSerializer.ToJson(OriginalContext.From(e.Metadata.OriginalContext)),
                EventData = PropertyBagSerializer.Serialize(e.Event,jsonSerializer) 
            }).ToList();

            var commit = new Commit
            {
                Id = 0,
                CorrelationId = eventStream.CorrelationId,
                CommitId =  eventStream.Id,
                Timestamp = eventStream.Timestamp.ToUnixTimeMilliseconds(),
                EventSourceId = eventStream.Source.EventSource,
                EventSourceArtifact = eventStream.Source.Artifact,
                CommitNumber = eventStream.Source.Version.Commit,
                Sequence = eventStream.Source.Version.Sequence,
                Events = events
            };

            return commit;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public  Dolittle.Runtime.Events.Store.CommittedEventStream ToCommittedEventStream(ISerializer serializer)
        {
            return new CommittedEventStream(
                (ulong)Id,
                new VersionedEventSource(new EventSourceVersion(this.CommitNumber,this.Sequence),new EventSourceKey(EventSourceId,EventSourceArtifact)),
                CommitId,
                CorrelationId,
                DateTimeOffset.FromUnixTimeMilliseconds(Timestamp),
                new EventStream(Events.Select(e => e.ToEventEnvelope(serializer)))
             );
        } 
    }   
}