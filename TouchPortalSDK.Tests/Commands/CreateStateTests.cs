using AutoFixture;
using AutoFixture.NUnit3;
using FakeItEasy;
using NUnit.Framework;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Commands;

public class CreateStateTests
{
    [Theory]
    [FakeItEasyData]
    public void Success(string stateId, string desc, string defaultValue, string parentGroup, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(true);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.CreateState(stateId, desc, defaultValue, parentGroup);

        Assert.That(result, Is.True);

        A.CallTo(() => socket.SendMessage(A<string>.That.Contains("\"createState\""))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(stateId))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(desc))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(defaultValue))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(parentGroup))).MustHaveHappened();
    }

    [Theory]
    [FakeItEasyData]
    public void Failed(string stateId, string desc, string defaultValue, string parentGroup, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(false);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.CreateState(stateId, desc, defaultValue, parentGroup);

        Assert.That(result, Is.False);
    }

    [Theory]
    [InlineFakeItEasyData("stateId", "", false)]
    [InlineFakeItEasyData("", "desc", false)]
    [InlineFakeItEasyData("stateId", "desc", true)]
    public void Ignored(string stateId, string desc, bool expected, IFixture fixture)
    {
        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.CreateState(stateId, desc);

        Assert.That(result, Is.EqualTo(expected));
    }
}