using AutoFixture;
using AutoFixture.NUnit3;
using FakeItEasy;
using NUnit.Framework;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Commands;

public class SettingsTests
{
    [Theory]
    [FakeItEasyData]
    public void Success(string stateId, string[] values, string instanceId, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(true);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.ChoiceUpdate(stateId, values, instanceId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);

            A.CallTo(() => socket.SendMessage(A<string>.That.Contains("\"choiceUpdate\""))).MustHaveHappened();
            A.CallTo(() => socket.SendMessage(A<string>.That.Contains(stateId))).MustHaveHappened();
        });
    }

    [Theory]
    [FakeItEasyData]
    public void Failed(string stateId, string[] values, string instanceId, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(false);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.ChoiceUpdate(stateId, values, instanceId);

        Assert.That(result, Is.False);
    }

    [Theory]
    [InlineFakeItEasyData("", false)]
    [InlineFakeItEasyData("choiceId", true)]
    public void Ignored(string choiceId, bool expected, IFixture fixture)
    {
        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.ChoiceUpdate(choiceId, default, default);

        Assert.That(result, Is.EqualTo(expected));
    }
}