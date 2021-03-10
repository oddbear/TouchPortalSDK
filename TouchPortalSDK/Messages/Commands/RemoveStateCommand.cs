using System;

namespace TouchPortalSDK.Messages.Commands
{
    public class RemoveStateCommand : ITouchPortalCommand
    {
        public string Type => "removeState";

        public string Id { get; }

        public RemoveStateCommand(string stateId)
        {
            if (string.IsNullOrWhiteSpace(stateId))
                throw new ArgumentNullException(nameof(stateId));

            Id = stateId;
        }

        public string GetKey()
            => Id;
    }
}
