namespace TouchPortalSDK.Interfaces
{
    public interface ITouchPortalEvent
    {
        /// <summary>
        /// Type of the message, see Touch Portal API documentation.
        /// </summary>
        string Type { get; }
    }
}
