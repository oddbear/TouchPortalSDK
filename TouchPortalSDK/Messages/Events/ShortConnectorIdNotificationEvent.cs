using System.Text.Json.Serialization;
using TouchPortalSDK.Configuration.Json;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models;

namespace TouchPortalSDK.Messages.Events
{
    public class ShortConnectorIdNotificationEvent : ITouchPortalMessage
    {
        public string Type { get; set; }
        public string PluginId { get; set; }
        public string ConnectorId { get; set; }

        [JsonConverter(typeof(StringValueConverter<ConnectorShortId>))]
        public ConnectorShortId ShortId { get; set; }

        public Identifier GetIdentifier()
            => new Identifier(Type, ConnectorId, default);
    }

    public class ConnectorShortId : ITypedId
    {
        public string Value { get; private set; }

        void ITypedId.SetValue(string shortId)
        {
            Value = shortId;
        }

        public override string ToString()
        {
            return Value?.ToString();
        }
    }
}
