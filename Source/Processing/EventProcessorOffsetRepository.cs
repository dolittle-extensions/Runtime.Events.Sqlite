/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 * --------------------------------------------------------------------------------------------*/

using System.Linq;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using System.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
namespace Dolittle.Runtime.Events.Sqlite.Processing
{
    /// <summary>
    /// A Sqlite implementation of <see cref="IEventProcessorOffsetRepository" />
    /// </summary>
    public class EventProcessorOffsetRepository : IEventProcessorOffsetRepository
    {
        private ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="logger"></param>
        public EventProcessorOffsetRepository(Database database, ILogger logger)
        {
            _logger = logger;
            _database = database;
        }

        #region IDisposable Support
        /// <summary>
        /// Disposed flag to detect redundant calls
        /// </summary>
        protected bool disposedValue = false;
        private readonly Database _database;

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~EventStore() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.

        /// <summary>
        /// Disposes of the EventStore
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        /// <inheritdoc />
        public CommittedEventVersion Get(EventProcessorId eventProcessorId)
        {
            var version = GetOffset(eventProcessorId);
            if(version == null)
                return CommittedEventVersion.None;
            
            return version.ToCommittedEventVersion();
        }

        /// <inheritdoc />
        public void Set(EventProcessorId eventProcessorId, CommittedEventVersion committedEventVersion)
        {
            using (var es = _database.GetContext())
            {
                //TODO: this can be optimized so that the update is the thing we expect most and the insert is the edge case
                var commandText = "INSERT OR REPLACE INTO Offsets (Id, Major, Minor, Revision) VALUES (@Id,@Major,@Minor,@Revision);";
                var id = new SqliteParameter("@Id", eventProcessorId.Value.ToString());
                var major = new SqliteParameter("@Major", committedEventVersion.Major);
                var minor = new SqliteParameter("@Minor", committedEventVersion.Minor);
                var revision = new SqliteParameter("@Revision", committedEventVersion.Revision);
                es.Database.ExecuteSqlCommand(commandText,id,major,minor,revision);
            }
        }

        Persistence.Offset GetOffset(EventProcessorId id)
        {
            using (var es = _database.GetContext())
            {
                return es.Offsets.SingleOrDefault(_ => _.Id == id.ToString());
            }
        }
    }
}