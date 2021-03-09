namespace TouchPortalSDK.Messages.Commands
{
    public class RemoveStateCommand : BaseCommand
    {
        public string Id { get; }

        public RemoveStateCommand(string stateId)
            : base("removeState")
        {
            Id = stateId;
        }
    }
}
