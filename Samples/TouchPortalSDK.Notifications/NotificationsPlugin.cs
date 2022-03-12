using Microsoft.Extensions.Logging;
using SampleUtils;
using System.Diagnostics;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Messages.Models;

namespace TouchPortalSDK.Notifications
{
    public class NotificationsPlugin : SamplePluginBase
    {
        protected override ILogger _logger { get;  }
        protected override ITouchPortalClient _client { get; }

        public NotificationsPlugin(ITouchPortalClientFactory clientFactory,
                            ILogger<NotificationsPlugin> logger)
        {
            _logger = logger;
            _client = clientFactory.Create(this);
        }

        public void Run()
        {
            //Connect to Touch Portal:
            _client.Connect();

            //Sends a notification to the Touch Portal UI this needs options the user can react on:
            _client.ShowNotification($"TouchPortalSDK.Notifications|update", "SamplePlugin: new version", "Please update to version 1.0!", new[] {
                new NotificationOptions { Id = "update", Title = "Update this plugin" },
                new NotificationOptions { Id = "readMore", Title = "Read more..." }
            });
        }

        public override void OnNotificationOptionClickedEvent(NotificationOptionClickedEvent message)
        {
            _logger.LogObjectAsJson(message);

            if (message.NotificationId is "TouchPortalSDK.Notifications|update")
            {
                switch (message.OptionId)
                {
                    //Example for opening a web browser (windows):
                    case "update":
                        Process.Start(new ProcessStartInfo
                        {
                            UseShellExecute = true,
                            FileName = "https://www.nuget.org/packages/TouchPortalSDK/"
                        });
                        break;
                    case "readMore":
                        Process.Start(new ProcessStartInfo
                        {
                            UseShellExecute = true,
                            FileName = "https://github.com/oddbear/TouchPortalSDK/"
                        });
                        break;
                }
            }
        }
    }
}
