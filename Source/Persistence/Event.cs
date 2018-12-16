using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Dolittle.PropertyBags;
using Dolittle.Serialization.Json;
using Dolittle.Artifacts;
using Dolittle.Collections;

namespace Dolittle.Runtime.Events.Sqlite.Persistence
{
    /// <summary>
    /// 
    /// </summary>
    public class Event
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long GlobalCommitNumber { get; set; }        
        /// <summary>
        /// 
        /// </summary>
        public Guid CorrelationId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid EventArtifact { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Generation { get; set; }
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
        public ulong Commit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public uint Sequence { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long Occurred { get; set; } 
        /// <summary>
        /// 
        /// </summary>
        public string OriginalContext { get; set; }  
        /// <summary>
        /// 
        /// </summary>
        public string EventData { get; set; }  

        /// <summary>
        ///
        /// </summary>
        /// <returns>An <see cref="EventEnvelope" /> instance corresponding to the <see cref="Event" /> representation</returns>
        public EventEnvelope ToEventEnvelope(ISerializer serializer)
        {
           return new EventEnvelope(ToEventMetadata(serializer),ToPropertyBag(serializer));
        }

        EventMetadata ToEventMetadata(ISerializer serializer)
        {
            return new EventMetadata(this.Id,ToVersionedEventSource(),this.CorrelationId,new Artifact(this.EventArtifact,this.Generation),DateTimeOffset.FromUnixTimeMilliseconds(Occurred), serializer.FromJson<Persistence.OriginalContext>(this.OriginalContext).ToOriginalContext());
        }

        PropertyBag ToPropertyBag(ISerializer serializer)
        {
            return PropertyBagSerializer.From(EventData, serializer);
        }

        VersionedEventSource ToVersionedEventSource()
        {
            return new VersionedEventSource(new EventSourceVersion(this.Commit,this.Sequence), new EventSourceKey(this.EventSourceId, this.EventSourceArtifact));
        }
    }
}