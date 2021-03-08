using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TouchPortalSDK.Sockets
{
    public interface ITouchPortalSocket
    {
        Func<string, Task> OnMessage { get; set; }
        Action<Exception> OnClose { get; set; }
        Task<bool> Connect();
        Task<string> Pair();
        bool Listen();
        Task<bool> SendMessage(Dictionary<string, object> message);
        Task<bool> SendMessage(string jsonMessage);
        void CloseSocket();
    }
}