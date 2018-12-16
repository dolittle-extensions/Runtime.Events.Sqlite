namespace Dolittle.Runtime.Events.Sqlite
{
    using Dolittle.Runtime.Events;
    using Dolittle.ResourceTypes;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the Sqlite implemenation of EventStore as a Resource
    /// </summary>
    public class EventStoreResourceTypeRepresentation : IRepresentAResourceType
    {
        static IDictionary<Type, Type> _bindings = new Dictionary<Type, Type>
        {
            {typeof(Dolittle.Runtime.Events.Store.IEventStore), typeof(Dolittle.Runtime.Events.Sqlite.Store.EventStore)},
            {typeof(Dolittle.Runtime.Events.Relativity.IGeodesics), typeof(Dolittle.Runtime.Events.Sqlite.Relativity.Geodesics)},
            {typeof(Dolittle.Runtime.Events.Processing.IEventProcessorOffsetRepository), typeof(Dolittle.Runtime.Events.Sqlite.Processing.EventProcessorOffsetRepository)}
        };
        
        /// <inheritdoc/>
        public ResourceType Type => "eventStore";
        /// <inheritdoc/>
        public ResourceTypeImplementation ImplementationName => "Sqlite";
        /// <inheritdoc/>
        public Type ConfigurationObjectType => typeof(EventStoreConfiguration);
        /// <inheritdoc/>
        public IDictionary<Type, Type> Bindings => _bindings;
    }
}