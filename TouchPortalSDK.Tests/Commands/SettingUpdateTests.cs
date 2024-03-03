﻿using AutoFixture;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Tests.Commands.Extensions;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Commands
{
    public class SettingUpdateTests
    {
        [Theory]
        [AutoMoqData]
        public void Success(string name, string value, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(true);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.SettingUpdate(name, value);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                var parameter = socket.SendMessage_Parameter();
                Assert.That(parameter, Does.Contain("\"settingUpdate\""));
                Assert.That(parameter, Does.Contain(name));
                Assert.That(parameter, Does.Contain(value));
            });
        }

        [Theory]
        [AutoMoqData]
        public void Failed(string name, string value, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(false);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.SettingUpdate(name, value);

            Assert.That(result, Is.False);
        }

        [Theory]
        [InlineAutoMoqData("", false)]
        [InlineAutoMoqData("name", true)]
        public void Ignored(string name, bool expected, IFixture fixture)
        {
            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.SettingUpdate(name);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
