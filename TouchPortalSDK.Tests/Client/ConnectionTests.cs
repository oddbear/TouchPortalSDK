using AutoFixture;
using AutoFixture.NUnit3;
using NUnit.Framework;
using System;
using System.Threading;
using FakeItEasy;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Tests.Client.Extensions;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Client;

public class ConnectionTests
{
    [Test]
    public void PluginId_Required()
    {
        var mock = A.Fake<ITouchPortalEventHandler>();
        A.CallTo(() => mock.PluginId).Returns(null as string);
        Assert.Throws<InvalidOperationException>(() => new TouchPortalClient(mock, default, default));
    }

    [Theory]
    [FakeItEasyData]
    public void Connection_Failed(IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.Connect()).Returns(false);

        ITouchPortalClient client = fixture.Create<TouchPortalClient>();
        var result = client.Connect();
        Assert.That(result, Is.False);

        A.CallTo(() => socket.Connect()).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void Pairing_Failed(IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.Connect()).Returns(true);
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(false);

        ITouchPortalClient client = fixture.Create<TouchPortalClient>();
        var result = client.Connect();
        Assert.That(result, Is.False);

        A.CallTo(() => socket.Connect()).MustHaveHappenedOnceExactly();
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void Listen_Failed(IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.Connect()).Returns(true);
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(true);
        A.CallTo(() => socket.Listen()).Returns(false);

        ITouchPortalClient client = fixture.Create<TouchPortalClient>();
        var result = client.Connect();
        Assert.That(result, Is.False);

        A.CallTo(() => socket.Connect()).MustHaveHappenedOnceExactly();
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() => socket.Listen()).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void Connection_Success(IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.Connect()).Returns(true);
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(true);
        A.CallTo(() => socket.Listen()).Returns(true);

        ITouchPortalClient client = fixture.Create<TouchPortalClient>();
        client.SetPrivate("_lastInfoEvent", new InfoEvent()); //final return to be true.
        client.SetPrivate("_infoWaitHandle", new ManualResetEvent(true)); //Thread not waiting for response.
        var result = client.Connect();
        Assert.That(result, Is.True);

        A.CallTo(() => socket.Connect()).MustHaveHappenedOnceExactly();
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() => socket.Listen()).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void Client_Close(IFixture fixture, [Frozen] ITouchPortalSocket socket, [Frozen] ITouchPortalEventHandler eventHandler)
    {
        ITouchPortalClient client = fixture.Create<TouchPortalClient>();
        client.Close();

        A.CallTo(() => socket.CloseSocket()).MustHaveHappenedOnceExactly();
        A.CallTo(() => eventHandler.OnClosedEvent("Closed by plugin.")).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [FakeItEasyData]
    public void MessageHandler_Close(IFixture fixture, [Frozen] ITouchPortalSocket socket, [Frozen] ITouchPortalEventHandler eventHandler)
    {
        IMessageHandler messageHandler = fixture.Create<TouchPortalClient>();
        messageHandler.Close("unitTest", new Exception());

        A.CallTo(() => socket.CloseSocket()).MustHaveHappenedOnceExactly();
        A.CallTo(() => eventHandler.OnClosedEvent("unitTest")).MustHaveHappenedOnceExactly();
    }
}