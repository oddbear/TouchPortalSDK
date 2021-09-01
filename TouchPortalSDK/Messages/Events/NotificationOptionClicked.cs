using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models;

namespace TouchPortalSDK.Messages.Events
{
    public class NotificationOptionClicked : ITouchPortalMessage
    {
        public string Type { get; set; }

        public string NotificationId { get; set; }

        public string OptionId { get; set; }

        public Identifier GetIdentifier()
            => new Identifier(Type, NotificationId, default);
    }
}
