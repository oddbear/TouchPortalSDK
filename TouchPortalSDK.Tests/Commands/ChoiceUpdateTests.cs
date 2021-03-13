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
    public class ChoiceUpdateTests
    {
        [Theory]
        [AutoMoqData]
        public void Success(string stateId, string[] values, string instanceId, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket, [Frozen] Mock<IStateManager> stateManager)
        {
            socket.SendMessage_Setup().Returns(true);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.ChoiceUpdate(stateId, values, instanceId);
            Assert.True(result);

            var parameter = socket.SendMessage_Parameter();
            StringAssert.Contains("\"choiceUpdate\"", parameter);
            StringAssert.Contains(stateId, parameter);

            stateManager.LogMessage_Verify(Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public void Failed(string stateId, string[] values, string instanceId, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket, [Frozen] Mock<IStateManager> stateManager)
        {
            socket.SendMessage_Setup().Returns(false);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.ChoiceUpdate(stateId, values, instanceId);
            Assert.False(result);

            stateManager.LogMessage_Verify(Times.Never);
        }

        [Theory]
        [InlineAutoMoqData("", false)]
        [InlineAutoMoqData("choiceId", true)]
        public void Ignored(string choiceId, bool expected, IFixture fixture)
        {
            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.ChoiceUpdate(choiceId, default, default);
            Assert.AreEqual(expected, result);
        }
    }
}
