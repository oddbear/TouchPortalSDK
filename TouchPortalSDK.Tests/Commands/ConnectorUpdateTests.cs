using AutoFixture;
using AutoFixture.NUnit3;
using FakeItEasy;
using NUnit.Framework;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Tests.Fixtures;
using TouchPortalSDK.Values;

namespace TouchPortalSDK.Tests.Commands;

public class ConnectorUpdateTests
{
    [Theory]
    [InlineFakeItEasyData("connectorId", 0)]
    [InlineFakeItEasyData("connectorId", 100)]
    public void Success(string connectorId, int value, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(true);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.ConnectorUpdate(connectorId, value);

        Assert.That(result, Is.True);

        A.CallTo(() => socket.SendMessage(A<string>.That.Contains("\"connectorUpdate\""))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(connectorId))).MustHaveHappened();
    }

    [Theory]
    [InlineFakeItEasyData("connectorId", -1)]
    [InlineFakeItEasyData("connectorId", 101)]
    [InlineFakeItEasyData("", 50)]
    public void Fails_Validation(string connectorId, int value, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(true);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.ConnectorUpdate(connectorId, value);

        Assert.That(result, Is.False);
    }

    [Theory]
    [InlineFakeItEasyData("shortId", 0)]
    [InlineFakeItEasyData("shortId", 100)]
    public void Success_ShortId(string shortId, int value, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(true);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.ConnectorUpdate(new ConnectorShortId(shortId), value);

        Assert.That(result, Is.True);

        A.CallTo(() => socket.SendMessage(A<string>.That.Contains("\"connectorUpdate\""))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(shortId))).MustHaveHappened();
    }

    [Theory]
    [InlineFakeItEasyData("shortId", -1)]
    [InlineFakeItEasyData("shortId", 101)]
    [InlineFakeItEasyData("", 50)]
    public void Fails_ShortId_Validation(string shortId, int value, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(true);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.ConnectorUpdate(new ConnectorShortId(shortId), value);

        Assert.That(result, Is.False);
    }
}