using System;
using System.Linq;
using Moq;
using Moq.Language.Flow;
using TouchPortalSDK.Messages.Items;
using TouchPortalSDK.Sockets;
using TouchPortalSDK.Utils;

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

    public static class MockStateManagerExtensions
    {
        public static void LogMessage_Verify(this Mock<IStateManager> stateManager, Func<Times> times)
            => stateManager.Verify(mock => mock.LogMessage(It.IsAny<Identifier>(), It.IsAny<string>()), times);
    }
}
