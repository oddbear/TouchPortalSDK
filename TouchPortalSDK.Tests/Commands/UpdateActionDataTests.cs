using AutoFixture;
using AutoFixture.NUnit3;
using NUnit.Framework;
using System.Globalization;
using FakeItEasy;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models.Enums;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Commands;

public class UpdateActionDataTests
{
    [Theory]
    [FakeItEasyData]
    public void Success(string dataId, double minValue, double maxValue, string instanceId, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(true);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.UpdateActionData(dataId, minValue, maxValue, ActionDataType.Number, instanceId);

        Assert.That(result, Is.True);

        A.CallTo(() => socket.SendMessage(A<string>.That.Contains("\"updateActionData\""))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(dataId))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(minValue.ToString(CultureInfo.InvariantCulture)))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(maxValue.ToString(CultureInfo.InvariantCulture)))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(instanceId))).MustHaveHappened();
    }

    [Theory]
    [FakeItEasyData]
    public void Failed(string dataId, double minValue, double maxValue, string instanceId, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(false);

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.UpdateActionData(dataId, minValue, maxValue, ActionDataType.Number, instanceId);
            
        Assert.That(result, Is.False);
    }

    [Theory]
    [InlineFakeItEasyData("", false)]
    [InlineFakeItEasyData("dataId", true)]
    public void Ignored(string dataId, bool expected, IFixture fixture)
    {
        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.UpdateActionData(dataId, default, default, default, default);
            
        Assert.That(expected, Is.EqualTo(result));
    }
}