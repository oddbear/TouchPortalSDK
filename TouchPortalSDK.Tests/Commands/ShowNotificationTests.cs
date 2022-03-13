using AutoFixture;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using System;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models;
using TouchPortalSDK.Tests.Commands.Extensions;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Commands
{
    public class ShowNotificationTests
    {
        [Theory]
        [AutoMoqData]
        public void Success(string notificationId, string title, string message, string optionId, string optionTitle, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(true);

            var notificationOptions = new[] {
                new NotificationOptions { Id = optionId, Title = optionTitle }
            };
            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.ShowNotification(notificationId, title, message, notificationOptions);
            Assert.True(result);

            var parameter = socket.SendMessage_Parameter();
            StringAssert.Contains("\"showNotification\"", parameter);
            StringAssert.Contains(notificationId, parameter);
            StringAssert.Contains(title, parameter);
            StringAssert.Contains(message, parameter);
        }

        [Theory]
        [AutoMoqData]
        public void Fails_Socket(string notificationId, string title, string message, string optionId, string optionTitle, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(false);

            var notificationOptions = new[] {
                new NotificationOptions { Id = optionId, Title = optionTitle }
            };
            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.ShowNotification(notificationId, title, message, notificationOptions);
            Assert.False(result);
        }

        [Theory]
        [InlineAutoMoqData("", "title", "message", "optionId", "optionTitle")]
        [InlineAutoMoqData("notificationId", "", "message", "optionId", "optionTitle")]
        [InlineAutoMoqData("notificationId", "title", "", "optionId", "optionTitle")]
        [InlineAutoMoqData("notificationId", "title", "message", "", "optionTitle")]
        [InlineAutoMoqData("notificationId", "title", "message", "optionId", "")]
        public void Fails_Validation(string notificationId, string title, string message, string optionId, string optionTitle, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(true);

            var notificationOptions = new[] {
                new NotificationOptions { Id = optionId, Title = optionTitle }
            };

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.ShowNotification(notificationId, title, message, notificationOptions);
            Assert.False(result);
        }

        [AutoMoqData]
        public void Fails_OptionsCount(string notificationId, string title, string message, IFixture fixture, [Frozen] Mock<ITouchPortalSocket> socket)
        {
            socket.SendMessage_Setup().Returns(true);

            var notificationOptions = Array.Empty<NotificationOptions>();

            ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
            var result = commandHandler.ShowNotification(notificationId, title, message, notificationOptions);
            Assert.False(result);
        }
    }
}
