namespace TouchPortalSDK.Interfaces
{
    public interface ITouchPortalSocket
    {
        /// <summary>
        /// Connects to TouchPortal.
        /// </summary>
        /// <returns>success flag</returns>
        bool Connect();

        /// <summary>
        /// Starts the listener thread, and listens for events from TouchPortal.
        /// </summary>
        /// <returns>success flag</returns>
        bool Listen();

        /// <summary>
        /// Sends a message to TouchPortal.
        /// </summary>
        /// <param name="jsonMessage"></param>
        /// <returns>success flag</returns>
        bool SendMessage(string jsonMessage);

        /// <summary>
        /// Closes the socket.
        /// </summary>
        void CloseSocket();
    }
}