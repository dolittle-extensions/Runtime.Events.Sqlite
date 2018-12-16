using Dolittle.Runtime.Events.Relativity;

namespace Dolittle.Runtime.Events.Sqlite.Persistence
{

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