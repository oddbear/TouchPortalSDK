namespace TouchPortalSDK.Sockets
{
    public interface ITouchPortalSocket
    {
        bool Connect();
        bool Listen();
        bool SendMessage(string jsonMessage);
        void CloseSocket();
    }
}