using System.Collections.Generic;
using TouchPortalSDK.Messages.Commands;
using TouchPortalSDK.Messages.States;

namespace TouchPortalSDK.Utils
{
    public class StateManager : IStateManager
    {
        private readonly Dictionary<string, State> _states;
        private readonly Dictionary<string, Choice> _choices;
        private readonly Dictionary<string, ActionData> _actionData;

        public IReadOnlyCollection<State> States
            => _states.Values;

        public IReadOnlyCollection<Choice> Choices
            => _choices.Values;

        public IReadOnlyCollection<ActionData> ActionData
            => _actionData.Values;

        public StateManager()
        {
            _states = new Dictionary<string, State>();
            _choices = new Dictionary<string, Choice>();
            _actionData = new Dictionary<string, ActionData>();
        }
        
        public void AddStateOfCommand(ITouchPortalCommand touchPortalCommand)
        {
            switch (touchPortalCommand)
            {
                case CreateStateCommand command:
                    _states[command.GetKey()] = new StateCreate(command.Id, command.Desc, command.DefaultValue);
                    break;
                case StateUpdateCommand command:
                    _states[command.GetKey()] = new State(command.Id, command.Value);
                    break;
                case RemoveStateCommand command:
                    _states.Remove(command.Id);
                    break;

                case ChoiceUpdateCommand command:
                    if (command.InstanceId is null) _choices.Clear();
                    _choices[command.GetKey()] = new Choice(command.Id, command.Value, command.InstanceId);
                    break;

                //Partly persisted in the page(.tml) file. Ex. %AppData%pages\(main).tml
                //After restart, old actions will be update, new ones will get the one from entry.tp
                case UpdateActionDataCommand command:
                    if (command.InstanceId is null) _actionData.Clear();
                    _actionData[command.GetKey()] = new ActionData(command.Data.Id, command.Data.MinValue, command.Data.MaxValue, command.Data.Type);
                    break;

                case PairCommand command:
                    break;

                //This is managed by the callback, and stored by TouchPortal in:
                //HKEY_CURRENT_USER\SOFTWARE\JavaSoft\Prefs\app\core\utilities\ <plugin name folder> \
                case SettingUpdateCommand command:
                    break;
            }
        }
    }
}
