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

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                var parameter = socket.SendMessage_Parameter();
                Assert.That(parameter, Does.Contain("\"updateActionData\""));
                Assert.That(parameter, Does.Contain(dataId));
                Assert.That(parameter, Does.Contain(minValue.ToString(CultureInfo.InvariantCulture)));
                Assert.That(parameter, Does.Contain(maxValue.ToString(CultureInfo.InvariantCulture)));
                Assert.That(parameter, Does.Contain(instanceId));
            });
        }

        [Theory]
        [AutoMoqData]
        public void Failed(string dataId, double minValue, double maxValue, string instanceId, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(false);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.UpdateActionData(dataId, minValue, maxValue, ActionDataType.Number, instanceId);
            
            Assert.That(result, Is.False);
        }

        [Theory]
        [InlineAutoMoqData("", false)]
        [InlineAutoMoqData("dataId", true)]
        public void Ignored(string dataId, bool expected, IFixture fixture)
        {
            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.UpdateActionData(dataId, default, default, default, default);
            
            Assert.That(expected, Is.EqualTo(result));
        }
    }
}
