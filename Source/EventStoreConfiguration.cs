using Dolittle.Lifecycle;

namespace Dolittle.Runtime.Events.Sqlite
{
    /// <summary>
    /// Defines the config values for the Sqlite instance
    /// </summary>
    [SingletonPerTenant]
    public class EventStoreConfiguration
    {
        /// <summary>
        /// Gets or set the Path for the Sqlite instance
        /// </summary>
        public string Path { get; set; }    
        /// <summary>
        /// Gets or set the name of the Database
        /// </summary>        
        public string DatabaseName { get; set; }
    }
}