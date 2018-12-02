namespace Dolittle.Runtime.Events.Processing.SqlLite.Specs
{
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.SqlLite.Processing;
    using Dolittle.Runtime.Events.Processing.InMemory.Specs;
    using Dolittle.Runtime.Events.SqlLite.Specs;

    public class SUTProvider : IProvideTheOffsetRepository
    {
        public IEventProcessorOffsetRepository Build() => new test_event_processor_offset_repository(new a_database());
    }
}