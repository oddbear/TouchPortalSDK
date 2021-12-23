using System.Collections.Generic;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models;

namespace TouchPortalSDK.Messages.Events
{
    public class ConnectorChangeEvent : ITouchPortalMessage
    {
        /// <summary>
        /// Touch Portal closes/stops the plugin or shuts down.
        /// </summary>
        public string Type { get; set; }

        public string PluginId { get; set; }

        public string ConnectorId { get; set; }

        public int Value { get; set; }

        public IReadOnlyCollection<ActionDataSelected> Data { get; set; }

        public Identifier GetIdentifier()
            => new Identifier(Type, ConnectorId, default);
    }
}
