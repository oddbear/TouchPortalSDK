namespace TouchPortalSDK.Messages.Commands
{
    public interface ITouchPortalCommand
    { 
        string Type { get; }

        string GetKey();
    }
}
