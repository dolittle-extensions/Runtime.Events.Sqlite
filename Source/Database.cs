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
        //static readonly LoggerFactory ConsoleLoggerFactory = new LoggerFactory(new[] {new ConsoleLoggerProvider((_, __) => true, true)});
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
                //.UseLoggerFactory(ConsoleLoggerFactory)
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
                    context.Database.ExecuteSqlCommand(BEFORE_TRIGGER);
                    context.Database.ExecuteSqlCommand(AFTER_TRIGGER);
                }
            }

            return new EventStoreContext(CreateOptions());
        }

//This needs to go, we don't need the distinction between Duplicate and Concurrency Issue
    const string BEFORE_TRIGGER = @"
CREATE TRIGGER IF NOT EXISTS insert_commit_id_check 
   BEFORE INSERT
   ON Commits
BEGIN
    SELECT
        CASE
            WHEN (SELECT Id FROM Commits WHERE CommitId == NEW.CommitId) IS NOT NULL THEN
            RAISE (FAIL,'CommitId is duplicate')
    END;
END;";   

    const string AFTER_TRIGGER = @"
CREATE TRIGGER IF NOT EXISTS insert_commit_version_check 
   AFTER INSERT
   ON Commits
BEGIN
    SELECT
        CASE
            WHEN (SELECT Id FROM Commits WHERE EventSourceId == NEW.EventSourceId AND EventSourceArtifact == NEW.EventSourceArtifact AND CommitNumber > NEW.CommitNumber) IS NOT NULL THEN
            RAISE (FAIL,'Version is older')
    END;
END;";        
    }
}
