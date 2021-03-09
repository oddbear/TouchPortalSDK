using System;

namespace TouchPortalSDK.Sockets
{
    public interface ITouchPortalSocket
    {
        bool Connect();
        bool Listen(Action<string> onMessageCallBack);
        bool SendMessage(string jsonMessage);
        void CloseSocket();
    }
}