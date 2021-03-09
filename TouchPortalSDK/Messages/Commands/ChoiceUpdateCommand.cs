using System;

namespace TouchPortalSDK.Messages.Commands
{
    public class ChoiceUpdateCommand : BaseCommand
    {
        public string Id { get; }

        public string[] Value { get; }

        public string InstanceId { get; }

        public ChoiceUpdateCommand(string listId, string[] value, string instanceId = null)
            : base("choiceUpdate")
        {
            Id = listId;
            Value = value ?? Array.Empty<string>();
            InstanceId = instanceId;
        }
    }
}