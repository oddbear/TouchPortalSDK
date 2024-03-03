using AutoFixture;
using AutoFixture.NUnit3;
using NUnit.Framework;
using System;
using FakeItEasy;
using TouchPortalSDK.Clients;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models;
using TouchPortalSDK.Tests.Fixtures;

namespace TouchPortalSDK.Tests.Commands;

public class ShowNotificationTests
{
    [Theory]
    [FakeItEasyData]
    public void Success(string notificationId, string title, string message, string optionId, string optionTitle, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(true);

        var notificationOptions = new[] {
            new NotificationOptions { Id = optionId, Title = optionTitle }
        };
        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.ShowNotification(notificationId, title, message, notificationOptions);

        Assert.That(result, Is.True);

        A.CallTo(() => socket.SendMessage(A<string>.That.Contains("\"showNotification\""))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(notificationId))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(title))).MustHaveHappened();
        A.CallTo(() => socket.SendMessage(A<string>.That.Contains(message))).MustHaveHappened();
    }

    [Theory]
    [FakeItEasyData]
    public void Fails_Socket(string notificationId, string title, string message, string optionId, string optionTitle, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(false);

        var notificationOptions = new[] {
            new NotificationOptions { Id = optionId, Title = optionTitle }
        };
        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.ShowNotification(notificationId, title, message, notificationOptions);

        Assert.That(result, Is.False);
    }

    [Theory]
    [InlineFakeItEasyData("", "title", "message", "optionId", "optionTitle")]
    [InlineFakeItEasyData("notificationId", "", "message", "optionId", "optionTitle")]
    [InlineFakeItEasyData("notificationId", "title", "", "optionId", "optionTitle")]
    [InlineFakeItEasyData("notificationId", "title", "message", "", "optionTitle")]
    [InlineFakeItEasyData("notificationId", "title", "message", "optionId", "")]
    public void Fails_Validation(string notificationId, string title, string message, string optionId, string optionTitle, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(true);

        var notificationOptions = new[] {
            new NotificationOptions { Id = optionId, Title = optionTitle }
        };

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.ShowNotification(notificationId, title, message, notificationOptions);

        Assert.That(result, Is.False);
    }

    [Theory]
    [FakeItEasyData]
    public void Fails_OptionsCount(string notificationId, string title, string message, IFixture fixture, [Frozen] ITouchPortalSocket socket)
    {
        A.CallTo(() => socket.SendMessage(A<string>.Ignored)).Returns(true);

        var notificationOptions = Array.Empty<NotificationOptions>();

        ICommandHandler commandHandler = fixture.Create<TouchPortalClient>();
        var result = commandHandler.ShowNotification(notificationId, title, message, notificationOptions);

        Assert.That(result, Is.False);
    }
}