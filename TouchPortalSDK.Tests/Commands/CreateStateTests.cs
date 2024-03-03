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
    public class CreateStateTests
    {
        [Theory]
        [AutoMoqData]
        public void Success(string stateId, string desc, string defaultValue, string parentGroup, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(true);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.CreateState(stateId, desc, defaultValue, parentGroup);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);

                var parameter = socket.SendMessage_Parameter();
                Assert.That(parameter, Does.Contain("\"createState\""));
                Assert.That(parameter, Does.Contain(stateId));
                Assert.That(parameter, Does.Contain(desc));
                Assert.That(parameter, Does.Contain(defaultValue));
                Assert.That(parameter, Does.Contain(parentGroup));
            });
        }

        [Theory]
        [AutoMoqData]
        public void Failed(string stateId, string desc, string defaultValue, string parentGroup, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(false);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.CreateState(stateId, desc, defaultValue, parentGroup);

            Assert.That(result, Is.False);
        }

        [Theory]
        [InlineAutoMoqData("stateId", "", false)]
        [InlineAutoMoqData("", "desc", false)]
        [InlineAutoMoqData("stateId", "desc", true)]
        public void Ignored(string stateId, string desc, bool expected, IFixture fixture)
        {
            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.CreateState(stateId, desc);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
