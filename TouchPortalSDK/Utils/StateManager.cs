using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Messages.Commands;

namespace TouchPortalSDK.Utils
{
    public class StateManager : IStateManager
    {
        private readonly ILogger<StateManager> _logger;
        private readonly Dictionary<(string id, string instantId), string> _commands;

        /// <inheritdoc cref="IStateManager"/>
        public IReadOnlyCollection<string> Commands => _commands.Values;
        
        public StateManager(ILogger<StateManager> logger = null)
        {
            _logger = logger;
            _commands = new Dictionary<(string, string), string>();
        }

        /// <inheritdoc cref="IStateManager"/>
        public void LogCommand<TCommand>(TCommand touchPortalCommand, string jsonCommand)
            where TCommand : ITouchPortalCommand
        {
            switch (touchPortalCommand)
            {
                case CreateStateCommand command:
                    _commands[(command.Id, null)] = jsonCommand;
                    break;
                case StateUpdateCommand command:
                    _commands[(command.Id, null)] = jsonCommand;
                    break;
                case RemoveStateCommand command:
                    _commands.Remove((command.Id, null));
                    break;

                case ChoiceUpdateCommand command:
                    ClearGarbage<TCommand>(command.Id, command.InstanceId);
                    _commands[(command.Id, command.InstanceId)] = jsonCommand;
                    break;

                //Partly persisted in the page(.tml) file. Ex. %AppData%pages\(main).tml
                //After restart, old actions will be update, new ones will get the one from entry.tp
                case UpdateActionDataCommand command:
                    ClearGarbage<TCommand>(command.Data.Id, command.InstanceId);
                    _commands[(command.Data.Id, command.InstanceId)] = jsonCommand;
                    break;

                case PairCommand command:
                    break;

                //This is managed by the callback, and stored by TouchPortal in:
                //HKEY_CURRENT_USER\SOFTWARE\JavaSoft\Prefs\app\core\utilities\ <plugin name folder> \
                case SettingUpdateCommand command:
                    break;
            }
        }

        private void ClearGarbage<TCommand>(string id, string instanceId)
        {
            //No need to clear, this command does not override anything.
            if (instanceId is null)
                return;

            var keys = _commands.Keys
                .Where(key => key.id == id)
                .Where(key => key.instantId != null)
                .ToArray();

            _logger.LogDebug($"Clearing out '{keys.Length}' commands that had it's state overriden by the current command '{typeof(TCommand).Name}' with id '{id}'.");

            //Clears out all with instanceId that is now overriden by a change to all instances:
            foreach (var key in keys)
                _commands.Remove(key);
        }
    }
}
