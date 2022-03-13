using TouchPortalSDK.Interfaces;

namespace TouchPortalSDK.Messages.Events
{
    public class CloseEvent : ITouchPortalEvent
    {
        /// <summary>
        /// Touch Portal closes/stops the plugin or shuts down.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The id of the plugin.
        /// </summary>
        public string PluginId { get; set; }
    }
}
