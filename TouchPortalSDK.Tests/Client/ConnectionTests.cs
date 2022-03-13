using AutoFixture;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Tests.Client.Extensions;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Client
{
    public class ConnectionTests
    {
        [Test]
        public void PluginId_Required()
        {
            var mock = new Mock<ITouchPortalEventHandler>();
            mock.SetupGet(mock => mock.PluginId).Returns(null as string);
            Assert.Throws<InvalidOperationException>(() => new TouchPortalClient(mock.Object, default, default));
        }

        [Theory]
        [AutoMoqData]
        public void Connection_Failed(IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.Setup(mock => mock.Connect()).Returns(false);

            ITouchPortalClient client = fixture.Create<TouchPortalClient>();
            var result = client.Connect();
            Assert.False(result);

            socket.Verify(mock => mock.Connect(), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Pairing_Failed(IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.Setup(mock => mock.Connect()).Returns(true);
            socket.Setup(mock => mock.SendMessage(It.IsAny<string>())).Returns(false);

            ITouchPortalClient client = fixture.Create<TouchPortalClient>();
            var result = client.Connect();
            Assert.False(result);

            socket.Verify(mock => mock.Connect(), Times.Once);
            socket.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Listen_Failed(IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.Setup(mock => mock.Connect()).Returns(true);
            socket.Setup(mock => mock.SendMessage(It.IsAny<string>())).Returns(true);
            socket.Setup(mock => mock.Listen()).Returns(false);

            ITouchPortalClient client = fixture.Create<TouchPortalClient>();
            var result = client.Connect();
            Assert.False(result);

            socket.Verify(mock => mock.Listen(), Times.Once);
            socket.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Once);
            socket.Verify(mock => mock.Listen(), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Connection_Success(IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.Setup(mock => mock.Connect()).Returns(true);
            socket.Setup(mock => mock.SendMessage(It.IsAny<string>())).Returns(true);
            socket.Setup(mock => mock.Listen()).Returns(true);

            ITouchPortalClient client = fixture.Create<TouchPortalClient>();
            client.SetPrivate("_lastInfoEvent", new InfoEvent()); //final return to be true.
            client.SetPrivate("_infoWaitHandle", new ManualResetEvent(true)); //Thread not waiting for response.
            var result = client.Connect();
            Assert.True(result);

            socket.Verify(mock => mock.Listen(), Times.Once);
            socket.Verify(mock => mock.SendMessage(It.IsAny<string>()), Times.Once);
            socket.Verify(mock => mock.Listen(), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Client_Close(IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket, [Frozen] Mock<ITouchPortalEventHandler> eventHandler)
        {
            ITouchPortalClient client = fixture.Create<TouchPortalClient>();
            client.Close();
            socket.Verify(mock => mock.CloseSocket(), Times.Once);
            eventHandler.Verify(mock => mock.OnClosedEvent("Closed by plugin."), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void MessageHandler_Close(IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket, [Frozen] Mock<ITouchPortalEventHandler> eventHandler)
        {
            IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
            messageHandler.Close("unitTest", new Exception());
            socket.Verify(mock => mock.CloseSocket(), Times.Once);
            eventHandler.Verify(mock => mock.OnClosedEvent("unitTest"), Times.Once);
        }
    }
}
