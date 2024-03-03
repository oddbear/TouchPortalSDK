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

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                var parameter = socket.SendMessage_Parameter();
                Assert.That(parameter, Does.Contain("\"connectorUpdate\""));
                Assert.That(parameter, Does.Contain(connectorId));
            });
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

            Assert.That(result, Is.False);
        }

        [Theory]
        [InlineAutoMoqData("shortId", 0)]
        [InlineAutoMoqData("shortId", 100)]
        public void Success_ShortId(string shortId, int value, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(true);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.ConnectorUpdate(new ConnectorShortId(shortId), value);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                var parameter = socket.SendMessage_Parameter();
                Assert.That(parameter, Does.Contain("\"connectorUpdate\""));
                Assert.That(parameter, Does.Contain(shortId));
            });
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

            Assert.That(result, Is.False);
        }
    }
}
