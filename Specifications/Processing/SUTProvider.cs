namespace Dolittle.Runtime.Events.Processing.Sqlite.Specs
{
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.Sqlite.Processing;
    using Dolittle.Runtime.Events.Processing.InMemory.Specs;
    using Dolittle.Runtime.Events.Sqlite.Specs;

    public class SUTProvider : IProvideTheOffsetRepository
    {
        public IEventProcessorOffsetRepository Build() => new test_event_processor_offset_repository(new a_database());
    }
}