namespace TouchPortalSDK.Messages.Commands
{
    public class StateUpdateCommand : BaseCommand
    {
        public string Id { get; }

        public string Value { get; }

        public StateUpdateCommand(string stateId, string value)
            : base("stateUpdate")
        {
            Id = stateId;
            Value = value ?? string.Empty;
        }
    }
}