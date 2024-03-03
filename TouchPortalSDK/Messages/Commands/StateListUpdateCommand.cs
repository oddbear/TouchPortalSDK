using System;
using TouchPortalSDK.Interfaces;

namespace TouchPortalSDK.Messages.Commands
{
    public class StateListUpdateCommand : ITouchPortalCommand
    {
        public string Type => "stateListUpdate";

        public string Id { get; set; }

        public string[] Value { get; set; }

        public static StateListUpdateCommand CreateAndValidate(string stateId, string[] values)
        {
            if (string.IsNullOrWhiteSpace(stateId))
                throw new ArgumentNullException(nameof(stateId));

            var command = new StateListUpdateCommand
            {
                Id = stateId,
                Value = values
            };

            return command;
        }
    }
}