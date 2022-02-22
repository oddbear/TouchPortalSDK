namespace TouchPortalSDK.Interfaces
{
    public interface ITouchPortalSocket
    {
        /// <summary>
        /// The connection state of the Socket being used internally.
        /// </summary>
        public bool IsConnected { get; }

        /// <summary>
        /// Connects to Touch Portal.
        /// </summary>
        /// <returns>success flag</returns>
        bool Connect();

        /// <summary>
        /// Starts the listener thread, and listens for events from Touch Portal.
        /// </summary>
        /// <returns>success flag</returns>
        bool Listen();

        /// <summary>
        /// Sends a message to Touch Portal.
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
