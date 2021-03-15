namespace TouchPortalSDK.Interfaces
{
    /// <summary>
    /// Factory interface for creating a TouchPortal client.
    /// </summary>
    public interface ITouchPortalClientFactory
    {
        /// <summary>
        /// Create a TouchPortal Client
        /// </summary>
        /// <param name="eventHandler">Handler the events from TouchPortal, normally the plugin instance.</param>
        /// <returns>TouchPortal Client</returns>
        ITouchPortalClient Create(ITouchPortalEventHandler eventHandler);
    }
}