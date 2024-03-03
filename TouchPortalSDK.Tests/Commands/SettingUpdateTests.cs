using AutoFixture;
using AutoFixture.NUnit3;
using FakeItEasy;
using NUnit.Framework;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Commands;

public class SettingUpdateTests
{
    [Theory]
    [FakeItEasyData]
    public void Success(string name, string value, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(true);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.SettingUpdate(name, value);

        Assert.That(result, Is.True);

        A.CallTo(() => socket.SendMessage(A<string>.That.Contains("\"settingUpdate\""))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(name))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(value))).MustHaveHappened();
    }

    [Theory]
    [FakeItEasyData]
    public void Failed(string name, string value, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(false);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.SettingUpdate(name, value);

        Assert.That(result, Is.False);
    }

    [Theory]
    [InlineFakeItEasyData("", false)]
    [InlineFakeItEasyData("name", true)]
    public void Ignored(string name, bool expected, IFixture fixture)
    {
        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.SettingUpdate(name);
        Assert.That(result, Is.EqualTo(expected));
    }
}