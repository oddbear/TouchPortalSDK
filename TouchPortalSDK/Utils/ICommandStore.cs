using System.Collections.Generic;

namespace TouchPortalSDK.Utils
{
    public interface ICommandStore
    {
        /// <summary>
        /// Method to log the commands sent during this session.
        /// </summary>
        /// <param name="stateFile">PluginId to generate filename for writing command log.</param>
        /// <param name="messages">All commands sent during this session.</param>
        void StoreCommands(string stateFile, IReadOnlyCollection<string> messages);

        /// <summary>
        /// Method to restore all state that was logged previously.
        /// </summary>
        /// <param name="stateFile">PluginId to generate filename for reading command log.</param>
        /// <returns>All messages in json format that has been logged previously.</returns>
        IReadOnlyCollection<string> LoadCommands(string stateFile);
    }
}