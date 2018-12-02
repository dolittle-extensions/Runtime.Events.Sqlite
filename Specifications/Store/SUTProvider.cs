using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Specs;
using Dolittle.Runtime.Events.SqlLite.Store;
using Dolittle.Security;
using Dolittle.Serialization.Json;
using Dolittle.Types;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.SqlLite.Specs.Store
{
    public class SUTProvider : IProvideTheEventStore
    {
        public IEventStore Build() => new test_event_store(new a_database());
    }

    public class test_event_store : EventStore
    {
        readonly a_database _database;

        public test_event_store(a_database db) : base(given.a_logger())
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