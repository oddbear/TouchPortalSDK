namespace TouchPortalSDK.Messages.Commands
{
    public class PairCommand : BaseCommand
    {
        public string Id { get; }

        public PairCommand(string pluginId)
            : base("pair")
        {
            Id = pluginId;
        }
    }
}
