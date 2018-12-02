using System;

namespace Dolittle.Runtime.Events.SqlLite.Specs
{
    public class a_database : IDisposable
    {
        public void Dispose()
        {
            ClearDatabase();
        }

        void ClearDatabase()
        {
            
        }
    }
}