using System;
using TouchPortalSDK.Interfaces;

namespace TouchPortalSDK.Messages.Commands
{
    public class ChoiceUpdateCommand : ITouchPortalCommand
    {
        public string Type => "choiceUpdate";

        public string Id { get; set; }

        public string[] Value { get; set; }

        public string InstanceId { get; set; }

        public static ChoiceUpdateCommand CreateAndValidate(string listId, string[] value, string instanceId = null)
        {
            if (string.IsNullOrWhiteSpace(listId))
                throw new ArgumentNullException(nameof(listId));

            var command = new ChoiceUpdateCommand
            {
                Id = listId,
                Value = value ?? Array.Empty<string>()
            };

            if (!string.IsNullOrWhiteSpace(instanceId))
                command.InstanceId = instanceId;

            return command;
        }
    }
}