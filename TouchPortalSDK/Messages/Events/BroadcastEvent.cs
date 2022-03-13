using TouchPortalSDK.Interfaces;

namespace TouchPortalSDK.Messages.Events
{
    public class BroadcastEvent : ITouchPortalEvent
    {
        /// <summary>
        /// Broadcast type is a global event all plugins will receive.
        /// In 2.3 the only event is og PageChange at the Device.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Event that was broadcast, ex. pageChange
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// Name of the page the device is currently on. Ex. "(main)"
        /// </summary>
        public string PageName { get; set; }
    }
}
