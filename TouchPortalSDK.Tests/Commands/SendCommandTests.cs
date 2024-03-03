﻿using AutoFixture;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Commands;
using TouchPortalSDK.Tests.Commands.Extensions;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Commands
{
    public class SendCommandTests
    {
        [Theory]
        [AutoMoqData]
        public void SendCommand(string message, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.SendMessage(message);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                var parameter = socket.SendMessage_Parameter();
                Assert.That(parameter, Is.EqualTo(message));
            });
        }

        [Theory]
        [AutoMoqData]
        public void PairCommand(string pluginId, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            var pair = new PairCommand(pluginId);
            var client = fixture.Create<TouchPortalClient>();
            client.SendCommand(pair);

            Assert.Multiple(() =>
            {
                var parameter = socket.SendMessage_Parameter();
                Assert.That(parameter, Does.Contain("\"pair\""));
                Assert.That(parameter, Does.Contain(pluginId));
            });
        }
    }
}
