using System.Collections.Generic;
using System.Linq;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models;

namespace TouchPortalSDK.Messages.Events
{
    public class ConnectorChangeEvent : ITouchPortalEvent
    {
        /// <summary>
        /// Touch Portal closes/stops the plugin or shuts down.
        /// </summary>
        public string Type { get; set; }

        public string PluginId { get; set; }

        public string ConnectorId { get; set; }

        public int Value { get; set; }

        public IReadOnlyCollection<Data> Data { get; set; }

        /// <summary>
        /// Indexer to get data values.
        /// </summary>
        /// <param name="dataId">the id of the datafield.</param>
        /// <returns>the value of the data field as string or null if not exists</returns>
        public string this[string dataId]
            => GetValue(dataId);

        /// <summary>
        /// Returns the value of the selected item in an action data field.
        /// This value can be null in some cases, and will be null if data field is miss written.
        /// </summary>
        /// <param name="dataId">the id of the datafield.</param>
        /// <returns>the value of the data field as string or null if not exists</returns>
        public string GetValue(string dataId)
            => Data?.SingleOrDefault(data => data.Id == dataId)?.Value;
    }
}
