using System.Linq;
using Moq;
using Moq.Language.Flow;
using TouchPortalSDK.Sockets;

namespace TouchPortalSDK.Tests.Commands.Extensions
{
    public static class MockSocketExtensions
    {
        public static ISetup<ITouchPortalSocket, bool> SendMessage_Setup(this Mock<ITouchPortalSocket> socket)
            => socket.Setup(mock => mock.SendMessage(It.IsAny<string>()));

        public static string SendMessage_Parameter(this Mock<ITouchPortalSocket> socket)
        {
            //Assumption:
            socket.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Once);

            var sendMessageInvocation = socket
                .Invocations
                .Single(invocation => invocation.Method.Name == nameof(ITouchPortalSocket.SendMessage));

            return sendMessageInvocation
                .Arguments
                .Single() as string;
        }
    }
}
