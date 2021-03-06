using System;
using System.Collections.Generic;

namespace TouchPortalSDK.Sockets
{
    public interface ITouchPortalSocket
    {
        Action<string> OnMessage { get; set; }
        Action<Exception> OnClose { get; set; }
        bool Connect();
        string Pair();
        bool Listen();
        bool SendMessage(Dictionary<string, object> message);
        bool SendMessage(string jsonMessage);
        void CloseSocket();
    }
}