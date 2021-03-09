using TouchPortalSDK.Messages.Events;

namespace TouchPortalSDK
{
    public interface ITouchPortalPlugin
    {
        /// <summary>
        /// Method to call when TouchPortal is connected.
        /// </summary>
        void OnInfo(InfoEvent message);

        /// <summary>
        /// Method to call when an item is selected from dropdown in Action Creation of a button.
        /// </summary>
        void OnListChanged(ListChangeEvent message);

        /// <summary>
        /// Method is called when an broadcast message is sent.
        /// </summary>
        void OnBroadcast(BroadcastEvent message);

        /// <summary>
        /// ...
        /// </summary>
        void OnSettings(SettingsEvent message);

        /// <summary>
        /// Method to call when a user presses a button on their device.
        /// </summary>
        void OnAction(ActionEvent message);

        /// <summary>
        /// Method to call when we loose connection to TouchPortal.
        /// </summary>
        void OnClosed();

        /// <summary>
        /// Messages that are unknown, and therefor we cannot deserialize to a known type.
        /// </summary>
        void OnUnhandled(string jsonMessage);
    }
}