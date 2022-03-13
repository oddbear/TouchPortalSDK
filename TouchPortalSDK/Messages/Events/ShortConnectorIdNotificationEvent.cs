using TouchPortalSDK.Interfaces;

namespace TouchPortalSDK.Messages.Events
{
    public class ShortConnectorIdNotificationEvent : ITouchPortalEvent
    {
        public string Type { get; set; }
        public string PluginId { get; set; }
        public string ConnectorId { get; set; }

        public string ShortId { get; set; }
    }
}
