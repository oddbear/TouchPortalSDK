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
            Assert.True(result);

            var parameter = socket.SendMessage_Parameter();
            StringAssert.Contains("\"createState\"", parameter);
            StringAssert.Contains(stateId, parameter);
            StringAssert.Contains(desc, parameter);
            StringAssert.Contains(defaultValue, parameter);
            StringAssert.Contains(defaultValue, parameter);
            StringAssert.Contains(parentGroup, parameter);
        }

        [Theory]
        [AutoMoqData]
        public void Failed(string stateId, string desc, string defaultValue, string parentGroup, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(false);

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.CreateState(stateId, desc, defaultValue, parentGroup);
            Assert.False(result);
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
