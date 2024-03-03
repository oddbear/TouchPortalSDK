using AutoFixture;
using AutoFixture.NUnit3;
using FakeItEasy;
using NUnit.Framework;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Commands;

public class StateUpdateTests
{
    [Theory]
    [FakeItEasyData]
    public void Success(string stateId, string value, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(true);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.StateUpdate(stateId, value);

        Assert.That(result, Is.True);

        A.CallTo(() => socket.SendMessage(A<string>.That.Contains("\"stateUpdate\""))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(stateId))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(value))).MustHaveHappened();
    }

    [Theory]
    [FakeItEasyData]
    public void Failed(string stateId, string value, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(false);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.StateUpdate(stateId, value);

        Assert.That(result, Is.False);
    }

    [Theory]
    [InlineFakeItEasyData("", false)]
    [InlineFakeItEasyData("stateId", true)]
    public void Ignored(string stateId, bool expected, IFixture fixture)
    {
        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.StateUpdate(stateId);

        Assert.That(result, Is.EqualTo(expected));
    }
}