using AutoFixture;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using TouchPortalSDK.Sockets;
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
            Assert.True(result);

            var parameter = socket.SendMessage_Parameter();
            StringAssert.Contains("\"settingUpdate\"", parameter);
            StringAssert.Contains(name, parameter);
            StringAssert.Contains(value, parameter);
        }

        [Theory]
        [AutoMoqData]
        public void Failed(string name, string value, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(false);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.SettingUpdate(name, value);
            Assert.False(result);
        }

        [Theory]
        [InlineAutoMoqData("", false)]
        [InlineAutoMoqData("name", true)]
        public void Ignored(string name, bool expected, IFixture fixture)
        {
            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.SettingUpdate(name);
            Assert.AreEqual(expected, result);
        }
    }
}
