using AutoFixture;
using AutoFixture.NUnit3;
using FakeItEasy;
using NUnit.Framework;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Commands;

public class RemoveStateTests
{
    [Theory]
    [FakeItEasyData]
    public void Success(string stateId, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(true);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.RemoveState(stateId);

        Assert.That(result, Is.True);

        A.CallTo(() => socket.SendMessage(A<string>.That.Contains("\"removeState\""))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(stateId))).MustHaveHappened();
    }

    [Theory]
    [FakeItEasyData]
    public void Failed(string stateId, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(false);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.RemoveState(stateId);

        Assert.That(result, Is.False);
    }

    [Theory]
    [InlineFakeItEasyData("", false)]
    [InlineFakeItEasyData("stateId", true)]
    public void Ignored(string stateId, bool expected, IFixture fixture)
    {
        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.RemoveState(stateId);

        Assert.That(result, Is.EqualTo(expected));
    }
}