using System;

namespace TouchPortalSDK.Messages.Commands
{
    public class ChoiceUpdateCommand : ITouchPortalCommand
    {
        public string Type => "choiceUpdate";

        public string Id { get; }

        public string[] Value { get; }

        public string InstanceId { get; }

        public ChoiceUpdateCommand(string listId, string[] value, string instanceId = null)
        {
            if (string.IsNullOrWhiteSpace(listId))
                throw new ArgumentNullException(nameof(listId));

            Id = listId;
            Value = value ?? Array.Empty<string>();

            if (!string.IsNullOrWhiteSpace(instanceId))
                InstanceId = instanceId;
        }

        public string GetKey()
            => InstanceId is null ? Id : $"{Id}:{InstanceId}";
    }
}