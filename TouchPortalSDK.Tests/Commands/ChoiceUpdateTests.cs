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
    public class SettingsTests
    {
        [Theory]
        [AutoMoqData]
        public void Success(string stateId, string[] values, string instanceId, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(true);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.ChoiceUpdate(stateId, values, instanceId);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                var parameter = socket.SendMessage_Parameter();
                Assert.That(parameter, Does.Contain("\"choiceUpdate\""));
                Assert.That(parameter, Does.Contain(stateId));
            });
        }

        [Theory]
        [AutoMoqData]
        public void Failed(string stateId, string[] values, string instanceId, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(false);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.ChoiceUpdate(stateId, values, instanceId);

            Assert.That(result, Is.False);
        }

        [Theory]
        [InlineAutoMoqData("", false)]
        [InlineAutoMoqData("choiceId", true)]
        public void Ignored(string choiceId, bool expected, IFixture fixture)
        {
            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.ChoiceUpdate(choiceId, default, default);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
