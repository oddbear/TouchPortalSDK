namespace TouchPortalSDK.Interfaces
{
    /// <summary>
    /// Factory interface for creating a TouchPortal socket.
    /// </summary>
    public interface ITouchPortalSocketFactory
    {
        /// <summary>
        /// Create a TouchPortal Socket
        /// </summary>
        /// <param name="messageHandler">Handler the json events from the Socket, normally the client instance.</param>
        /// <returns>TouchPortal Socket</returns>
        ITouchPortalSocket Create(IMessageHandler messageHandler);
    }
}