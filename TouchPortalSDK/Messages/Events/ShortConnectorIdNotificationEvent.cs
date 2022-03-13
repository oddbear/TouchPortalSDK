using System.Text.Json.Serialization;
using TouchPortalSDK.Interfaces;

namespace TouchPortalSDK.Messages.Events
{
    public class ShortConnectorIdNotificationEvent : ITouchPortalEvent
    {
        /// <summary>
        /// Plugin settings changed in TouchPortal UI.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The id of the plugin.
        /// </summary>
        public string PluginId { get; set; }

        /// <summary>
        /// This is the internal connectorId in Touch Portal,
        ///  starting with "pc_pluginId_" and ends with all the parameters in sequential order.
        /// </summary>
        [JsonPropertyName("connectorId")]
        public string TouchPortalConnectorId { get; set; }

        /// <summary>
        /// The internal shortId of Touch Portal.
        /// This id is unique per internal connectorId (see TouchPortalConnectorId).
        /// </summary>
        public string ShortId { get; set; }
    }
}
