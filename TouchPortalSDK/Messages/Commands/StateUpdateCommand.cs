using System;

namespace TouchPortalSDK.Messages.Commands
{
    public class StateUpdateCommand : ITouchPortalCommand
    {
        public string Type => "stateUpdate";

        public string Id { get; }

        public string Value { get; }

        public StateUpdateCommand(string stateId, string value)
        {
            if (string.IsNullOrWhiteSpace(stateId))
                throw new ArgumentNullException(nameof(stateId));

            Id = stateId;
            Value = value ?? string.Empty;
        }
    }
}