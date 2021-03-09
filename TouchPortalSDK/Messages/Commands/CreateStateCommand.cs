namespace TouchPortalSDK.Messages.Commands
{
    public class CreateStateCommand : BaseCommand
    {
        public string Id { get; }
        public string Desc { get; }
        public string DefaultValue { get; }

        public CreateStateCommand(string stateId, string displayName, string defaultValue)
            : base("createState")
        {
            Id = stateId;
            Desc = displayName;
            DefaultValue = defaultValue ?? string.Empty;
        }
    }
}
