using System.Collections.Generic;
using TouchPortalSDK.Messages.States;

namespace TouchPortalSDK.Utils
{
    public interface IStateManager
    {
        IReadOnlyCollection<State> States { get; }

        IReadOnlyCollection<Choice> Choices { get; }

        IReadOnlyCollection<ActionData> ActionData { get; }
    }
}