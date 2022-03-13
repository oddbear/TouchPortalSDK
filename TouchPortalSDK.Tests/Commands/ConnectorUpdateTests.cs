using AutoFixture;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Tests.Commands.Extensions;
using TouchPortalSDK.Tests.Fixtures;
using TouchPortalSDK.Values;

namespace TouchPortalSDK.Tests.Commands
{
    public class ConnectorUpdateTests
    {
        [Theory]
        [InlineAutoMoqData("connectorId", 0)]
        [InlineAutoMoqData("connectorId", 100)]
        public void Success(string connectorId, int value, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(true);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.ConnectorUpdate(connectorId, value);
            Assert.True(result);

            var parameter = socket.SendMessage_Parameter();
            StringAssert.Contains("\"connectorUpdate\"", parameter);
            StringAssert.Contains(connectorId, parameter);
        }

        [Theory]
        [InlineAutoMoqData("connectorId", -1)]
        [InlineAutoMoqData("connectorId", 101)]
        [InlineAutoMoqData("", 50)]
        public void Fails_Validation(string connectorId, int value, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(true);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.ConnectorUpdate(connectorId, value);
            Assert.False(result);
        }

        [Theory]
        [InlineAutoMoqData("shortId", 0)]
        [InlineAutoMoqData("shortId", 100)]
        public void Success_ShortId(string shortId, int value, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(true);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.ConnectorUpdate(new ConnectorShortId(shortId), value);
            Assert.True(result);

            var parameter = socket.SendMessage_Parameter();
            StringAssert.Contains("\"connectorUpdate\"", parameter);
            StringAssert.Contains(shortId, parameter);
        }

        [Theory]
        [InlineAutoMoqData("shortId", -1)]
        [InlineAutoMoqData("shortId", 101)]
        [InlineAutoMoqData("", 50)]
        public void Fails_ShortId_Validation(string shortId, int value, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(true);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.ConnectorUpdate(new ConnectorShortId(shortId), value);
            Assert.False(result);
        }
    }
}
