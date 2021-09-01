using System;
using System.Collections.Generic;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models;

namespace TouchPortalSDK.Messages.Commands
{
    public class ShowNotificationCommand : ITouchPortalMessage
    {
        public string Type => "showNotification";

        public string NotificationId { get; set; }

        public string Title { get; set; }

        public string Msg { get; set; }

        public NotificationOptions[] Options { get; set; }

        public ShowNotificationCommand(string notificationId, string title, string message)
        {
            if (string.IsNullOrWhiteSpace(notificationId))
                throw new ArgumentNullException(nameof(notificationId));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException(nameof(title));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            //if(!options.Any())
            //  throw new Exception("At least one option is required.");

            NotificationId = notificationId;
            Title = title;
            Msg = message;

            //TODO: parameter array:
            Options = new[] {
                new NotificationOptions { Id = "option 1", Title = "Option 1" },
                new NotificationOptions { Id = "option 2", Title = "Option 2" }
            };
        }

        /// <inheritdoc cref="ITouchPortalMessage" />
        public Identifier GetIdentifier()
            => new Identifier(Type, NotificationId, default);
    }
}
