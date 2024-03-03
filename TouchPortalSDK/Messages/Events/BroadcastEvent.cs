using TouchPortalSDK.Interfaces;

namespace TouchPortalSDK.Messages.Events
{
    public class BroadcastEvent : ITouchPortalEvent
    {
        /// <summary>
        /// Broadcast type is a global event all plugins will receive.
        /// In 2.3 the only event is og PageChange at the Device.
        /// </summary>
        public string Type { get; set; } = null!;

        /// <summary>
        /// Event that was broadcast, ex. pageChange
        /// </summary>
        public string Event { get; set; } = null!;

        /// <summary>
        /// Name of the page the device is currently on. Ex. "(main)"
        /// </summary>
        public string? PageName { get; set; }

        /// <summary>
        /// The name of the page navigated from.
        /// The value will be sent only when the broadcast is of the type "pageChange". 
        /// </summary>
        public string? PreviousPageName { get; set; }

        /// <summary>
        /// The device ip of the device navigating pages.
        /// The value will be sent only when the broadcast is of the type "pageChange". 
        /// </summary>
        public string? DeviceIp { get; set; }

        /// <summary>
        /// The device name of the device navigating pages.
        /// The value will be sent only when the broadcast is of the type "pageChange". 
        /// </summary>
        public string? DeviceName { get; set; }
    }
}
