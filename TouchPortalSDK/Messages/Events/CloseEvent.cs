namespace TouchPortalSDK.Messages.Events
{
    public class CloseEvent : BaseEvent
    {
        /// <summary>
        /// The id of the plugin.
        /// </summary>
        public string PluginId { get; set; }
    }
}
