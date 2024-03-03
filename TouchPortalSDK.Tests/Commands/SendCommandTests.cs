using AutoFixture;
using AutoFixture.NUnit3;
using FakeItEasy;
using NUnit.Framework;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Commands;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Commands;

public class SendCommandTests
{
    [Theory]
    [FakeItEasyData]
    public void SendCommand(string message, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.SendMessage(message);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);

            A.CallTo(() => socket.SendMessage(A<string>.That.Contains(message))).MustHaveHappened();
        });
    }

    [Theory]
    [FakeItEasyData]
    public void PairCommand(string pluginId, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        var pair = new PairCommand(pluginId);
        var client = fixture.Create<TouchPortalClient>();
        client.SendCommand(pair);

        A.CallTo(() => socket.SendMessage(A<string>.That.Contains("\"pair\""))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(pluginId))).MustHaveHappened();
    }
}