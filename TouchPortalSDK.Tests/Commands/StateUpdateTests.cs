using AutoFixture;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using TouchPortalSDK.Sockets;
using TouchPortalSDK.Tests.Commands.Extensions;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Commands
{
    public class StateUpdateTests
    {
        [Theory]
        [AutoMoqData]
        public void Success(string stateId, string value, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(true);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.StateUpdate(stateId, value);
            Assert.True(result);

            var parameter = socket.SendMessage_Parameter();
            StringAssert.Contains("\"stateUpdate\"", parameter);
            StringAssert.Contains(stateId, parameter);
            StringAssert.Contains(value, parameter);
        }

        [Theory]
        [AutoMoqData]
        public void Failed(string stateId, string value, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(false);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.StateUpdate(stateId, value);
            Assert.False(result);
        }

        [Theory]
        [InlineAutoMoqData("", false)]
        [InlineAutoMoqData("stateId", true)]
        public void Ignored(string stateId, bool expected, IFixture fixture)
        {
            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.StateUpdate(stateId);
            Assert.AreEqual(expected, result);
        }
    }
}
