using Dolittle.Runtime.Events.Relativity;
using Dolittle.Runtime.Events.Sqlite.Specs;

namespace Dolittle.Runtime.Events.Sqlite.Relativity.Specs
{
    internal class test_geodesics : Geodesics
    {
        private a_database _database;

        public test_geodesics(a_database db) : base(db,given.a_logger())
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