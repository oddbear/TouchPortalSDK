using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models;

namespace TouchPortalSDK.Messages.Events
{
    public class CloseEvent : ITouchPortalMessage
    {
        /// <summary>
        /// TouchPortal closes/stops the plugin or shuts down.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The id of the plugin.
        /// </summary>
        public string PluginId { get; set; }

        /// <inheritdoc cref="ITouchPortalMessage" />
        public Identifier GetIdentifier()
            => new Identifier(Type, PluginId, default);
    }
}
