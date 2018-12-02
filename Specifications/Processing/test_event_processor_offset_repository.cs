namespace Dolittle.Runtime.Events.Processing.SqlLite.Specs
{
    using Dolittle.Runtime.Events.SqlLite.Processing;
    using Dolittle.Runtime.Events.SqlLite.Specs;

    public class test_event_processor_offset_repository : EventProcessorOffsetRepository
    {
        private readonly a_database _database;

        public test_event_processor_offset_repository(a_database db): base(given.a_logger())
        {
            _database = db;
        }
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _database.Dispose();
                disposedValue = true;
            }
        }
    }
}