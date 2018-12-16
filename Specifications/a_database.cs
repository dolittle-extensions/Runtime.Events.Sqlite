using System;

namespace Dolittle.Runtime.Events.Sqlite.Specs
{
    public class a_database : Database, IDisposable
    {

        public a_database() : base(new EventStoreConfiguration(){ DatabaseName = "test.db", Path = Environment.CurrentDirectory})
        {

        }

        public override void Dispose()
        {
            DeleteDb();
            base.Dispose();
        }

        void DeleteDb()
        {
            using (var context = GetContext())
            { 
               context.Database.EnsureDeleted();
            }
        }
    }
}