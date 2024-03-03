using Microsoft.Extensions.Logging;
using SampleUtils;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;

namespace TouchPortalSDK.Settings
{
    public class SettingsPlugin : SamplePluginBase
    {
        protected override ILogger _logger { get; }
        protected override ITouchPortalClient _client { get; }

        public SettingsPlugin(
            ITouchPortalClientFactory clientFactory,
            ILogger<SettingsPlugin> logger)
        {
            _logger = logger;
            _client = clientFactory.Create(this);
        }

        public void Run()
        {
            //Connect to Touch Portal:
            _client.Connect();

            _client.SettingUpdate("Second", DateTime.UtcNow.ToString("s"));
        }

        public override void OnInfoEvent(InfoEvent message)
        {
            var settings = message.Settings;
            _logger.LogObjectAsJson(settings);
        }

        public override void OnSettingsEvent(SettingsEvent message)
        {
            var settings = message.Values;
            _logger.LogObjectAsJson(settings);
        }
    }
}
