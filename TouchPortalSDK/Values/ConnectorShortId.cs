using TouchPortalSDK.Messages.Events;

namespace TouchPortalSDK.Values
{
    public class ConnectorShortId
    {
        public string Value { get; }

        public ConnectorShortId(ShortConnectorIdNotificationEvent message)
        {
            Value = message.ShortId;
        }

        public override string ToString()
        {
            return Value?.ToString();
        }
    }
}
