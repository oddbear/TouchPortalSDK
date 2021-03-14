using TouchPortalSDK.Messages.Items;

namespace TouchPortalSDK.Messages
{
    public interface ITouchPortalMessage
    { 
        /// <summary>
        /// Type of the message, see TouchPortal API documentation.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets a unique identifier for a command/event.
        /// (Type, Id, Instance)
        /// </summary>
        /// <returns></returns>
        Identifier GetIdentifier();
    }
}
