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
    public class Offset : Value<Offset>
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }  

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public ulong Major { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public ulong Minor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public uint Revision { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventProcessorId"></param>
        /// <param name="committedEventVersion"></param>
        /// <returns></returns>
        public static Offset From(EventProcessorId eventProcessorId, CommittedEventVersion committedEventVersion)
        {
            return new Offset { Id = eventProcessorId.ToString(), 
                                Major = committedEventVersion.Major, 
                                Minor = committedEventVersion.Minor, 
                                Revision = committedEventVersion.Revision  };
        }   

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CommittedEventVersion ToCommittedEventVersion() 
        {
            return new CommittedEventVersion(Major,Minor,Revision);
        }
    }
}