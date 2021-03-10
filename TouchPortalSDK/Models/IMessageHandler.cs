using System;

namespace TouchPortalSDK.Models
{
    public interface IMessageHandler
    {
        /// <summary>
        /// Method for handling raw messages events, normally in json format.
        /// </summary>
        /// <param name="message"></param>
        void OnMessage(string message);

        /// <summary>
        /// Method for forcefully closing the message handler.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Close(string message, Exception exception);
    }
}