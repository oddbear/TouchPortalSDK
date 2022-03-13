using System;
using TouchPortalSDK.Interfaces;

namespace TouchPortalSDK.Messages.Commands
{
    public class StateUpdateCommand : ITouchPortalCommand
    {
        public string Type => "stateUpdate";

        public string Id { get; set; }

        public string Value { get; set; }

        public static StateUpdateCommand CreateAndValidate(string stateId, string value)
        {
            if (string.IsNullOrWhiteSpace(stateId))
                throw new ArgumentNullException(nameof(stateId));

            var command = new StateUpdateCommand
            {
                Id = stateId,
                Value = value ?? string.Empty
            };

            return command;
        }
    }
}