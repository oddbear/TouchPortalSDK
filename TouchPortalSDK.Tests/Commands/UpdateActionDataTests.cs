using AutoFixture;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using System.Globalization;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models.Enums;
using TouchPortalSDK.Tests.Commands.Extensions;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Commands
{
    public class UpdateActionDataTests
    {
        [Theory]
        [AutoMoqData]
        public void Success(string dataId, double minValue, double maxValue, string instanceId, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(true);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.UpdateActionData(dataId, minValue, maxValue, ActionDataType.Number, instanceId);
            Assert.True(result);

            var parameter = socket.SendMessage_Parameter();
            StringAssert.Contains("\"updateActionData\"", parameter);
            StringAssert.Contains(dataId, parameter);
            StringAssert.Contains(minValue.ToString(CultureInfo.InvariantCulture), parameter);
            StringAssert.Contains(maxValue.ToString(CultureInfo.InvariantCulture), parameter);
            StringAssert.Contains(instanceId, parameter);
        }

        [Theory]
        [AutoMoqData]
        public void Failed(string dataId, double minValue, double maxValue, string instanceId, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(false);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.UpdateActionData(dataId, minValue, maxValue, ActionDataType.Number, instanceId);
            Assert.False(result);
        }

        [Theory]
        [InlineAutoMoqData("", false)]
        [InlineAutoMoqData("dataId", true)]
        public void Ignored(string dataId, bool expected, IFixture fixture)
        {
            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.UpdateActionData(dataId, default, default, default, default);
            Assert.AreEqual(expected, result);
        }
    }
}
