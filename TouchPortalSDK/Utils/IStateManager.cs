using System.Collections.Generic;
using TouchPortalSDK.Messages.Commands;

namespace TouchPortalSDK.Utils
{
    public interface IStateManager
    {
        /// <summary>
        /// Property to get all commands sent during this session.
        /// </summary>
        IReadOnlyCollection<string> Commands { get; }

        /// <summary>
        /// Log a command that can be restored after a restart.
        /// </summary>
        /// <param name="touchPortalCommand"></param>
        /// <param name="jsonCommand"></param>
        void LogCommand<TCommand>(TCommand touchPortalCommand, string jsonCommand)
            where TCommand : ITouchPortalCommand;
    }
}