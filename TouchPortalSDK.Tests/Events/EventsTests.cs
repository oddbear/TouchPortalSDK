using AutoFixture;
using AutoFixture.NUnit3;
using FakeItEasy;
using NUnit.Framework;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Messages.Models;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Events;

public class EventsTests
{
    [Theory]
    [FakeItEasyData]
    public void InfoEvent(IFixture fixture, [Frozen] ITouchPortalEventHandler eventHandler)
    {
        IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
        messageHandler.OnMessage("{\"type\":\"info\"}");

        A.CallTo(() => eventHandler.OnInfoEvent(A<InfoEvent>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void CloseEvent(IFixture fixture, [Frozen] ITouchPortalEventHandler eventHandler)
    {
        IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
        messageHandler.OnMessage("{\"type\":\"closePlugin\"}");

        A.CallTo(() => eventHandler.OnClosedEvent("TouchPortal sent a Plugin close event.")).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void ListChangeEvent(IFixture fixture, [Frozen] ITouchPortalEventHandler eventHandler)
    {
        IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
        messageHandler.OnMessage("{\"type\":\"listChange\"}");

        A.CallTo(() => eventHandler.OnListChangedEvent(A<ListChangeEvent>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void BroadcastEvent(IFixture fixture, [Frozen] ITouchPortalEventHandler eventHandler)
    {
        IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
        messageHandler.OnMessage("{\"type\":\"broadcast\"}");

        A.CallTo(() => eventHandler.OnBroadcastEvent(A<BroadcastEvent>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void SettingsEvent(IFixture fixture, [Frozen] ITouchPortalEventHandler eventHandler)
    {
        IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
        messageHandler.OnMessage("{\"type\":\"settings\"}");

        A.CallTo(() => eventHandler.OnSettingsEvent(A<SettingsEvent>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void ActionEvent_Action(IFixture fixture, [Frozen] ITouchPortalEventHandler eventHandler)
    {
        IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
        messageHandler.OnMessage("{\"type\":\"action\"}");

        A.CallTo(() => eventHandler.OnActionEvent(A<ActionEvent>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void ActionEvent_Up(IFixture fixture, [Frozen] ITouchPortalEventHandler eventHandler)
    {
        IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
        messageHandler.OnMessage("{\"type\":\"up\"}");

        A.CallTo(() => eventHandler.OnActionEvent(A<ActionEvent>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void ActionEvent_Down(IFixture fixture, [Frozen] ITouchPortalEventHandler eventHandler)
    {
        IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
        messageHandler.OnMessage("{\"type\":\"down\"}");

        A.CallTo(() => eventHandler.OnActionEvent(A<ActionEvent>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void NotificationOptionClickedEvent(IFixture fixture, [Frozen] ITouchPortalEventHandler eventHandler)
    {
        IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
        messageHandler.OnMessage("{\"type\":\"notificationOptionClicked\"}");

        A.CallTo(() => eventHandler.OnNotificationOptionClickedEvent(A<NotificationOptionClickedEvent>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void ConnectorChangeEvent(IFixture fixture, [Frozen] ITouchPortalEventHandler eventHandler)
    {
        IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
        messageHandler.OnMessage("{\"type\":\"connectorChange\"}");

        A.CallTo(() => eventHandler.OnConnecterChangeEvent(A<ConnectorChangeEvent>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void ShortConnectorIdNotificationEvent(IFixture fixture, [Frozen] ITouchPortalEventHandler eventHandler)
    {
        IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
        messageHandler.OnMessage("{\"type\":\"shortConnectorIdNotification\"}");

        A.CallTo(() => eventHandler.OnShortConnectorIdNotificationEvent(A<ConnectorInfo>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void UnhandledEvent(IFixture fixture, [Frozen] ITouchPortalEventHandler eventHandler)
    {
        IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
        messageHandler.OnMessage("{\"type\":\"unknown\"}");

        A.CallTo(() => eventHandler.OnUnhandledEvent(A<string>.Ignored)).MustHaveHappenedOnceExactly();
    }
}