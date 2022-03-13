using System.Collections.Generic;
using System.Linq;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Values;

namespace TouchPortalSDK.Messages.Models
{
    public class ConnectorInfo
    {
        public ShortConnectorIdNotificationEvent NotificationEvent { get; set; }

        /// <summary>
        /// The ConnectorId as known from entry.tp
        /// </summary>
        public string ConnectorId { get; set; }

        /// <summary>
        /// The ShortId from Touch Portal.
        /// </summary>
        public ConnectorShortId ShortId { get; set; }

        /// <summary>
        /// The data permutation for this particular shortId.
        /// </summary>
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

        public ConnectorInfo(ShortConnectorIdNotificationEvent message)
        {
            NotificationEvent = message;

            try
            {
                ShortId = new ConnectorShortId(message.ShortId);
                var prefix = $"pc_{message.PluginId}_";
                var segments = message.TouchPortalConnectorId
                    .Substring(prefix.Length)
                    .Split('|');

                ConnectorId = segments[0];

                Data = segments
                    .Skip(1)
                    .Select(segment => segment.Split('='))
                    .Select(pair => Models.Data.Create(pair[0], pair[1]))
                    .ToArray();
            }
            catch
            {
                //Ignore, this original message is set to the  NotificationEvent property.
            }
        }
    }
}
