using System;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models;

namespace TouchPortalSDK.Messages.Commands
{
    internal class ShowNotificationCommand : ITouchPortalCommand
    {
        public string Type => "showNotification";

        public string NotificationId { get; set; }

        public string Title { get; set; }

        public string Msg { get; set; }

        public NotificationOptions[] Options { get; set; }

        public static ShowNotificationCommand CreateAndValidate(string notificationId, string title, string message, NotificationOptions[] notificationOptions)
        {
            if (string.IsNullOrWhiteSpace(notificationId))
                throw new ArgumentNullException(nameof(notificationId));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException(nameof(title));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            if (notificationOptions is null)
                throw new ArgumentNullException(nameof(notificationOptions));

            if (notificationOptions.Length == 0)
              throw new Exception("At least one option is required.");

            var command = new ShowNotificationCommand
            {
                NotificationId = notificationId,
                Title = title,
                Msg = message,

                Options = notificationOptions
            };

            return command;
        }
    }
}
