using System;
using System.Data.Common;
using System.IO;
using Dolittle.Concepts;
using Dolittle.Lifecycle;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Relativity;
using Dolittle.Runtime.Events.Store;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Dolittle.Runtime.Events.Sqlite.Persistence
{

    /// <summary>
    /// 
    /// </summary>
    public partial class GeodesicsOffset : Value<Offset>
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }  

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public ulong Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static GeodesicsOffset From(EventHorizonKey key, ulong offset)
        {
            return new GeodesicsOffset { Id = key.AsId(), Value = offset };
        }   
    }
}