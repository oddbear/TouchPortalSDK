using AutoFixture;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using TouchPortalSDK.Sockets;
using TouchPortalSDK.Tests.Commands.Extensions;
using TouchPortalSDK.Tests.Fixtures;
using TouchPortalSDK.Utils;

namespace TouchPortalSDK.Tests.Commands
{
    public class CreateStateTests
    {
        [Theory]
        [AutoMoqData]
        public void Success(string stateId, string desc, string defaultValue, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket, [Frozen] Mock<IStateManager> stateManager)
        {
            socket.SendMessage_Setup().Returns(true);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.CreateState(stateId, desc, defaultValue);
            Assert.True(result);
            
            var parameter = socket.SendMessage_Parameter();
            StringAssert.Contains("\"createState\"", parameter);
            StringAssert.Contains(stateId, parameter);
            StringAssert.Contains(desc, parameter);
            StringAssert.Contains(defaultValue, parameter);

            stateManager.LogMessage_Verify(Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Failed(string stateId, string desc, string defaultValue, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket, [Frozen] Mock<IStateManager> stateManager)
        {
            socket.SendMessage_Setup().Returns(false);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.CreateState(stateId, desc, defaultValue);
            Assert.False(result);
            
            stateManager.LogMessage_Verify(Times.Never);
        }

        [Theory]
        [InlineAutoMoqData("stateId", "", false)]
        [InlineAutoMoqData("", "desc", false)]
        [InlineAutoMoqData("stateId", "desc", true)]
        public void Ignored(string stateId, string desc, bool expected, IFixture fixture)
        {
            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.CreateState(stateId, desc);
            Assert.AreEqual(expected, result);
        }
    }
}
