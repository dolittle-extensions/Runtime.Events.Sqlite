using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.IO;
using Dolittle.Lifecycle;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Internal;

namespace Dolittle.Runtime.Events.Sqlite
{

    /// <summary>
    /// A DbContext covering the entities in the event store
    /// </summary>
    public class EventStoreContext : DbContext
	{
        /// <summary>
        /// Collection of Commits
        /// </summary>
        /// <value></value>
		public DbSet<Dolittle.Runtime.Events.Sqlite.Persistence.Commit> Commits { get; set; }
        /// <summary>
        /// Collection of Events
        /// </summary>
        /// <value></value>
		public DbSet<Dolittle.Runtime.Events.Sqlite.Persistence.Event> Events { get; set; }
        /// <summary>
        /// Collection of Offsets
        /// </summary>
        /// <value></value>
		public DbSet<Dolittle.Runtime.Events.Sqlite.Persistence.Offset> Offsets { get; set; }
        /// <summary>
        /// Collection of Offsets
        /// </summary>
        /// <value></value>
		public DbSet<Dolittle.Runtime.Events.Sqlite.Persistence.GeodesicsOffset> GeodesicsOffsets { get; set; }        

        //private readonly string _db;
        /// <summary>
        /// The connection to the DB
        /// </summary>
        protected SqliteConnection Connection { get; }

        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        public EventStoreContext()
        {
            Database.AutoTransactionsEnabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public EventStoreContext(DbContextOptions<EventStoreContext> options) : base(options)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            BuildCommitModel(modelBuilder);
            BuildEventModel(modelBuilder);
            BuildOffsetModel(modelBuilder);
            BuildGeodesicsModel(modelBuilder);

        }

        void BuildCommitModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Persistence.Commit>(_ => 
            {
                _.HasKey(c => c.Id);
                _.Property(c => c.Id).ValueGeneratedOnAdd();
                _.HasAlternateKey(c => c.CommitId);
                _.HasAlternateKey(c => new { c.EventSourceId, c.CommitNumber, c.Sequence });
                _.Property<Guid>(c => c.CorrelationId);
                _.Property<long>(c => c.Timestamp);
                _.Property<Guid>(c => c.EventSourceArtifact);
                _.Property<ulong>(c => c.CommitNumber);
                _.Property<uint>(c => c.Sequence);
                _.HasMany(c => c.Events);
            });
        }

        void BuildEventModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Persistence.Event>(_ => 
            {
                _.HasKey(e => e.Id);
                _.Property<Guid>(e => e.CorrelationId);
                _.Property<Guid>(e => e.EventSourceArtifact);
                _.Property<int>(e => e.Generation);
                _.Property<Guid>(e => e.EventSourceId);
                _.Property<Guid>(e => e.EventSourceArtifact);
                _.Property<ulong>(e => e.Commit);
                _.Property<uint>(e => e.Sequence);
                _.Property<string>( e => e.OriginalContext);
                _.Property<dynamic>(e => e.EventData);
            });
        }


        void BuildOffsetModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Persistence.Offset>(_ => 
            {
                _.HasKey(c => c.Id);
                _.Property<ulong>(c => c.Major);
                _.Property<ulong>(c => c.Minor);
                _.Property<uint>(c => c.Revision);
            });
        }

        void BuildGeodesicsModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Persistence.Offset>(_ => 
            {
                _.HasKey(c => c.Id);
                _.Property<ulong>(c => c.Major);
                _.Property<ulong>(c => c.Minor);
                _.Property<uint>(c => c.Revision);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> FromSql(string sql, IDictionary<string, object> parameters)
        {
            using (var cmd = this.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = sql;
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
         
                foreach (KeyValuePair<string, object> param in parameters)
                {
                    DbParameter dbParameter = cmd.CreateParameter();
                    dbParameter.ParameterName = param.Key;
                    dbParameter.Value = param.Value;
                    cmd.Parameters.Add(dbParameter);
                }
         
                var result = new List<dynamic>();
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var instance = new ExpandoObject() as IDictionary<string, object>;
                        for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                            instance.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
         
                        result.Add((ExpandoObject)instance);
                    }
                }
         
                return result;
            }
        }
	}
}