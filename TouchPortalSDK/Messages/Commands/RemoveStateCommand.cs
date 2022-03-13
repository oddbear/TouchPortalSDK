using System;
using TouchPortalSDK.Interfaces;

namespace TouchPortalSDK.Messages.Commands
{
    public class RemoveStateCommand : ITouchPortalCommand
    {
        public string Type => "removeState";

        public string Id { get; set; }

        public static RemoveStateCommand CreateAndValidate(string stateId)
        {
            if (string.IsNullOrWhiteSpace(stateId))
                throw new ArgumentNullException(nameof(stateId));

            var command = new RemoveStateCommand
            {
                Id = stateId
            };

            return command;
        }
    }
}
