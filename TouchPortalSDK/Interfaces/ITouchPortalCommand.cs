namespace TouchPortalSDK.Interfaces
{
    public interface ITouchPortalCommand
    {
        /// <summary>
        /// Type of the message, see Touch Portal API documentation.
        /// </summary>
        string Type { get; }
    }
}
