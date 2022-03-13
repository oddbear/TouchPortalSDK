using AutoFixture;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Messages.Models;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Events
{
    public class EventsTests
    {
        [Theory]
        [AutoMoqData]
        public void InfoEvent(IFixture fixture, [Frozen]Mock<ITouchPortalEventHandler> eventHandler)
        {
            IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
            messageHandler.OnMessage("{\"type\":\"info\"}");

            eventHandler.Verify(mock => mock.OnInfoEvent(It.IsAny<InfoEvent>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void CloseEvent(IFixture fixture, [Frozen] Mock<ITouchPortalEventHandler> eventHandler)
        {
            IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
            messageHandler.OnMessage("{\"type\":\"closePlugin\"}");

            eventHandler.Verify(mock => mock.OnClosedEvent("TouchPortal sent a Plugin close event."), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void ListChangeEvent(IFixture fixture, [Frozen] Mock<ITouchPortalEventHandler> eventHandler)
        {
            IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
            messageHandler.OnMessage("{\"type\":\"listChange\"}");

            eventHandler.Verify(mock => mock.OnListChangedEvent(It.IsAny<ListChangeEvent>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void BroadcastEvent(IFixture fixture, [Frozen] Mock<ITouchPortalEventHandler> eventHandler)
        {
            IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
            messageHandler.OnMessage("{\"type\":\"broadcast\"}");

            eventHandler.Verify(mock => mock.OnBroadcastEvent(It.IsAny<BroadcastEvent>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void SettingsEvent(IFixture fixture, [Frozen] Mock<ITouchPortalEventHandler> eventHandler)
        {
            IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
            messageHandler.OnMessage("{\"type\":\"settings\"}");

            eventHandler.Verify(mock => mock.OnSettingsEvent(It.IsAny<SettingsEvent>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void ActionEvent_Action(IFixture fixture, [Frozen] Mock<ITouchPortalEventHandler> eventHandler)
        {
            IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
            messageHandler.OnMessage("{\"type\":\"action\"}");

            eventHandler.Verify(mock => mock.OnActionEvent(It.IsAny<ActionEvent>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void ActionEvent_Up(IFixture fixture, [Frozen] Mock<ITouchPortalEventHandler> eventHandler)
        {
            IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
            messageHandler.OnMessage("{\"type\":\"up\"}");

            eventHandler.Verify(mock => mock.OnActionEvent(It.IsAny<ActionEvent>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void ActionEvent_Down(IFixture fixture, [Frozen] Mock<ITouchPortalEventHandler> eventHandler)
        {
            IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
            messageHandler.OnMessage("{\"type\":\"down\"}");

            eventHandler.Verify(mock => mock.OnActionEvent(It.IsAny<ActionEvent>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void NotificationOptionClickedEvent(IFixture fixture, [Frozen] Mock<ITouchPortalEventHandler> eventHandler)
        {
            IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
            messageHandler.OnMessage("{\"type\":\"notificationOptionClicked\"}");

            eventHandler.Verify(mock => mock.OnNotificationOptionClickedEvent(It.IsAny<NotificationOptionClickedEvent>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void ConnectorChangeEvent(IFixture fixture, [Frozen] Mock<ITouchPortalEventHandler> eventHandler)
        {
            IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
            messageHandler.OnMessage("{\"type\":\"connectorChange\"}");

            eventHandler.Verify(mock => mock.OnConnecterChangeEvent(It.IsAny<ConnectorChangeEvent>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void ShortConnectorIdNotificationEvent(IFixture fixture, [Frozen] Mock<ITouchPortalEventHandler> eventHandler)
        {
            IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
            messageHandler.OnMessage("{\"type\":\"shortConnectorIdNotification\"}");

            eventHandler.Verify(mock => mock.OnShortConnectorIdNotificationEvent(It.IsAny<ConnectorInfo>()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void UnhandledEvent(IFixture fixture, [Frozen] Mock<ITouchPortalEventHandler> eventHandler)
        {
            IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
            messageHandler.OnMessage("{\"type\":\"unknown\"}");

            eventHandler.Verify(mock => mock.OnUnhandledEvent(It.IsAny<string>()), Times.Once);
        }
    }
}
