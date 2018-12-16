using System;
using System.Data.Common;
using System.IO;
using Dolittle.Lifecycle;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Dolittle.Runtime.Events.Sqlite
{
    /// <summary>
    /// An instance of the database
    /// </summary>
    [SingletonPerTenant]  
    public class Database : IDisposable
    {
        static readonly LoggerFactory ConsoleLoggerFactory = new LoggerFactory(new[] {new ConsoleLoggerProvider((_, __) => true, true)});
        //string _db;
        DbConnection _connection;

        /// <summary>
        /// Handles the config info and creates a connection to the database
        /// </summary>
        /// <param name="config">Config needed to instantiate the correct connection to the database</param>
        public Database(EventStoreConfiguration config)
        {
        }

        DbContextOptions<EventStoreContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<EventStoreContext>()
                .UseSqlite(_connection)
                .UseLoggerFactory(ConsoleLoggerFactory)
                .Options;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public EventStoreContext GetContext() 
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection("DataSource={db}");
                _connection.Open();

                var options = CreateOptions();
                using (var context = new EventStoreContext(options))
                {
                    context.Database.EnsureCreated();
                }
            }

            return new EventStoreContext(CreateOptions());
        }
    }
}