using TouchPortalSDK.Messages.Events;

namespace TouchPortalSDK
{
    /// <summary>
    /// Interface used to register a plugin that can handle events from TouchPortal.
    /// </summary>
    public interface ITouchPortalEventHandler
    {
        /// <summary>
        /// EventHandler must define a pluginId to receive plugin events.
        /// </summary>
        public string PluginId { get; }

        /// <summary>
        /// Method to call when TouchPortal is connected.
        /// </summary>
        void OnInfoEvent(InfoEvent message);

        /// <summary>
        /// Method to call when an item is selected from dropdown in Action Creation of a button.
        /// </summary>
        void OnListChangedEvent(ListChangeEvent message);

        /// <summary>
        /// Method is called when an broadcast message is sent.
        /// </summary>
        void OnBroadcastEvent(BroadcastEvent message);

        /// <summary>
        /// Settings are first received as a part of the OnInfoEvent.
        /// Then updated through this event if either user changes a setting in TouchPortal, or the SettingUpdate is successfully triggered.
        /// </summary>
        void OnSettingsEvent(SettingsEvent message);

        /// <summary>
        /// Method to call when a user presses a button on their device.
        /// </summary>
        void OnActionEvent(ActionEvent message);

        /// <summary>
        /// Method to call when we loose connection to TouchPortal.
        /// </summary>
        /// <param name="message"></param>
        void OnClosedEvent(string message);

        /// <summary>
        /// Messages that are unknown, and therefor we cannot deserialize to a known type.
        /// </summary>
        void OnUnhandledEvent(string jsonMessage);
    }
}