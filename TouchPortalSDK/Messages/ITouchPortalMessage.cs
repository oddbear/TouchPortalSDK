using TouchPortalSDK.Messages.Items;

namespace TouchPortalSDK.Messages
{
    public interface ITouchPortalMessage
    { 
        string Type { get; }

        Identifier GetIdentifier();
    }
}
