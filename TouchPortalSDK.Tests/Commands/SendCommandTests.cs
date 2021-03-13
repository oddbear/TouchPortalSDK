using AutoFixture;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using TouchPortalSDK.Messages.Commands;
using TouchPortalSDK.Sockets;
using TouchPortalSDK.Tests.Commands.Extensions;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Commands
{
    public class SendCommandTests
    {
        [Theory]
        [AutoMoqData]
        public void SendCommand(string message, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.SendMessage(message);
            Assert.True(result);
            
            var parameter = socket.SendMessage_Parameter();
            Assert.AreEqual(message, parameter);
        }

        [Theory]
        [AutoMoqData]
        public void PairCommand(string pluginId, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            var pair = new PairCommand(pluginId);
            var client = fixture.Create<TouchPortalClient>();
            client.SendCommand(pair);

            var parameter = socket.SendMessage_Parameter();
            StringAssert.Contains("\"pair\"", parameter);
            StringAssert.Contains(pluginId, parameter);
        }
    }
}
