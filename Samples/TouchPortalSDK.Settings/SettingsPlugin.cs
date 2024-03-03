using Microsoft.Extensions.Logging;
using SampleUtils;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;

namespace TouchPortalSDK.Settings
{
    public class SettingsPlugin : SamplePluginBase
    {
        protected override ILogger Logger { get; }
        protected override ITouchPortalClient Client { get; }

        public SettingsPlugin(
            ITouchPortalClientFactory clientFactory,
            ILogger<SettingsPlugin> logger)
        {
            Logger = logger;
            Client = clientFactory.Create(this);
        }

        public void Run()
        {
            // Connect to Touch Portal:
            Client.Connect();

            Client.SettingUpdate("Second", DateTime.UtcNow.ToString("s"));
        }

        public override void OnInfoEvent(InfoEvent message)
        {
            var settings = message.Settings;
            Logger.LogObjectAsJson(settings);
        }

        public override void OnSettingsEvent(SettingsEvent message)
        {
            var settings = message.Values;
            Logger.LogObjectAsJson(settings);
        }
    }
}
