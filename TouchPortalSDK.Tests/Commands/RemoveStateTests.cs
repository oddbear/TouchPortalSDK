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

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                var parameter = socket.SendMessage_Parameter();
                Assert.That(parameter, Does.Contain("\"removeState\""));
                Assert.That(parameter, Does.Contain(stateId));
            });
        }

        [Theory]
        [AutoMoqData]
        public void Failed(string stateId, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(false);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.RemoveState(stateId);

            Assert.That(result, Is.False);
        }

        [Theory]
        [InlineAutoMoqData("", false)]
        [InlineAutoMoqData("stateId", true)]
        public void Ignored(string stateId, bool expected, IFixture fixture)
        {
            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.RemoveState(stateId);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
