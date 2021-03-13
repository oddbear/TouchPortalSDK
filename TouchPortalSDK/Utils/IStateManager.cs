using System.Collections.Generic;
using TouchPortalSDK.Messages.Items;

namespace TouchPortalSDK.Utils
{
    public interface IStateManager
    {
        /// <summary>
        /// Property to get all commands sent during this session.
        /// </summary>
        IReadOnlyCollection<string> Messages { get; }

        /// <summary>
        /// Log a command that can be restored after a restart.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="jsonCommand"></param>
        void LogMessage(Identifier identifier, string jsonCommand);
    }
}