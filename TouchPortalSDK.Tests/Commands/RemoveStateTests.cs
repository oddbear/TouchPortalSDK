using AutoFixture;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Tests.Commands.Extensions;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Commands
{
    public class RemoveStateTests
    {
        [Theory]
        [AutoMoqData]
        public void Success(string stateId, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(true);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.RemoveState(stateId);
            Assert.True(result);

            var parameter = socket.SendMessage_Parameter();
            StringAssert.Contains("\"removeState\"", parameter);
            StringAssert.Contains(stateId, parameter);
        }

        [Theory]
        [AutoMoqData]
        public void Failed(string stateId, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(false);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.RemoveState(stateId);
            Assert.False(result);
        }

        [Theory]
        [InlineAutoMoqData("", false)]
        [InlineAutoMoqData("stateId", true)]
        public void Ignored(string stateId, bool expected, IFixture fixture)
        {
            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.RemoveState(stateId);
            Assert.AreEqual(expected, result);
        }
    }
}
