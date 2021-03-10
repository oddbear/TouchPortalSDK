using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TouchPortalSDK.Configuration;
using TouchPortalSDK.Messages.States;

namespace TouchPortalSDK.Utils
{
    public class CommandStateStorage
    {
        //private readonly ILogger<CommandStateStorage> _logger;
        
        public void Store(StateManager stateManager)
        {
            try
            {
                var json = JsonSerializer.Serialize(stateManager, Options.JsonSerializerOptions);
                File.WriteAllText("customstates.json", json);
            }
            catch (Exception exception)
            {
                //_logger?.LogError(exception, "Could not store state.");
            }
        }

        public PrevStates Load()
        {
            try
            {
                var json = File.ReadAllText("customstates.json");
                return JsonSerializer.Deserialize<PrevStates>(json, Options.JsonSerializerOptions) ?? new PrevStates();
            }
            catch (Exception exception)
            {
                //_logger?.LogError(exception, "Could not load state.");
                return new PrevStates();
            }
        }

        public class PrevStates : IStateManager
        {
            public IReadOnlyCollection<State> States { get; }
            public IReadOnlyCollection<Choice> Choices { get; }
            public IReadOnlyCollection<ActionData> ActionData { get; }

            public PrevStates(IReadOnlyCollection<State> states = default,
                              IReadOnlyCollection<Choice> choices = default,
                              IReadOnlyCollection<ActionData> actionData = default)
            {
                States = states ?? Array.Empty<State>();
                Choices = choices ?? Array.Empty<Choice>();
                ActionData = actionData ?? Array.Empty<ActionData>();
            }
        }
    }
}
