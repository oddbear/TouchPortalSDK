using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Messages.Items;

namespace TouchPortalSDK.Utils
{
    public class StateManager : IStateManager
    {
        private readonly ILogger<StateManager> _logger;
        private readonly Dictionary<Identifier, string> _messages;

        /// <inheritdoc cref="IStateManager"/>
        public IReadOnlyCollection<string> Messages => _messages.Values;
        
        public StateManager(ILogger<StateManager> logger = null)
        {
            _logger = logger;
            _messages = new Dictionary<Identifier, string>();
        }

        /// <inheritdoc cref="IStateManager"/>
        public void LogMessage(Identifier identifier, string message)
        {
            ClearGarbage(identifier);

            switch (identifier.Type)
            {
                case "createState":
                case "stateUpdate":
                case "choiceUpdate":
                //Partly persisted in the page(.tml) file. Ex. %AppData%pages\(main).tml
                //After restart, old actions will be update, new ones will get the one from entry.tp
                case "updateActionData":
                    _messages[identifier] = message;
                    break;
                case "removeState":
                    _messages.Remove(identifier);
                    break;

                case "pair":
                    break;

                //This is managed by the callback, and stored by TouchPortal in:
                //HKEY_CURRENT_USER\SOFTWARE\JavaSoft\Prefs\app\core\utilities\ <plugin name folder> \
                case "settingUpdate":
                    break;
            }
        }

        /// <summary>
        /// States that are now overriden by this command, and no longer exists.
        /// </summary>
        /// <param name="identifier"></param>
        private void ClearGarbage(Identifier identifier)
        {
            //No need to clear, this command does not override anything.
            if (identifier.InstanceId is null)
                return;

            //Types with InstanceId:
            if (identifier.Type != "choiceUpdate" || identifier.InstanceId != "updateActionData")
                return;

            var keys = _messages.Keys
                .Where(key => key.Id == identifier.Id)
                .Where(key => key.InstanceId != null)
                .ToArray();

            _logger.LogDebug($"Clearing out '{keys.Length}' commands that had it's state overriden by the current command '{identifier.Type}' with id '{identifier.Id}'.");

            //Clears out all with instanceId that is now overriden by a change to all instances:
            foreach (var key in keys)
                _messages.Remove(key);
        }
    }
}
