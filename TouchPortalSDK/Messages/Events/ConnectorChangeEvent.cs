using System.Collections.Generic;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models;

namespace TouchPortalSDK.Messages.Events
{
    public class ConnectorChangeEvent : ITouchPortalMessage
    {
        public string Type { get; set; }

        public string PluginId { get; set; }

        public string ConnectorId { get; set; }
        
        public string Value { get; set; }

        public IReadOnlyCollection<ConnecterData> Data { get; set; }

        public Identifier GetIdentifier()
            => new Identifier(Type, ConnectorId, default);
    }
}
