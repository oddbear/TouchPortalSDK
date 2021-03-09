namespace TouchPortalSDK.Messages.Commands
{
    public class BaseCommand
    {
        public string Type { get; }

        protected BaseCommand(string type)
        {
            Type = type;
        }
    }
}
