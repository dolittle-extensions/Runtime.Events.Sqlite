/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 * --------------------------------------------------------------------------------------------*/

namespace Dolittle.Runtime.Events.Sqlite.Relativity
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Dolittle.Logging;
    using Dolittle.Runtime.Events.Relativity;
    using Dolittle.Runtime.Events.Store;
    using Dolittle.Serialization.Json;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// An Sqlite implementation of <see cref="IGeodesics" />
    /// </summary>
    public class Geodesics : IGeodesics
    {
        private readonly ILogger _logger;
        private readonly Database _database;

        /// <summary>
        /// Instantiates an instance of <see cref="IGeodesics" />
        /// </summary>
        /// <param name="database"></param>
        /// <param name="logger">A logger instance</param>
        public Geodesics(Database database, ILogger logger)
        {
            _logger = logger;
            _database = database;
        }

        /// <inheritdoc />
        public ulong GetOffset(EventHorizonKey eventHorizon)
        {
            using (var es = _database.GetContext())
            {
                return es.GeodesicsOffsets.SingleOrDefault(_ => _.Id == eventHorizon.AsId())?.Value ?? 0;
            }
        }

        /// <inheritdoc />
        public void SetOffset(EventHorizonKey key, ulong offset)
        {
            using (var es = _database.GetContext())
            {
                //TODO: this can be optimized so that the update is the thing we expect most and the insert is the edge case
                var commandText = "INSERT OR REPLACE INTO GeodesicsOffsets (Id, Value) VALUES (@Id,@Value);";
                var id = new SqliteParameter("@Id", key.AsId());
                var value = new SqliteParameter("@Value", offset);
                es.Database.ExecuteSqlCommand(commandText,id,value);
            }
        }

        #region IDisposable Support
        /// <summary>
        /// Disposed flag to detect redundant calls
        /// </summary>
        protected bool disposedValue = false;

        /// <summary>
        /// Disposes of managed and unmanaged resources
        /// </summary>
        /// <param name="disposing"></param>
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
    }

    /// <summary>
    /// Extension methods for the <see cref="EventHorizonKey"/>
    /// </summary>
    public static class EventHorizonKeyExtensions
    {
        /// <summary>
        /// Gets the id representation of the <see cref="EventHorizonKey"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AsId(this EventHorizonKey key) 
        {
            return key.Application.Value.ToString() + "-" + key.BoundedContext.Value.ToString();
        }
    }    
}