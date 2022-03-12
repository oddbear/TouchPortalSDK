using Microsoft.Extensions.Logging;
using SampleUtils;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;

namespace TouchPortalSDK.Notifications
{
    public class ConnectorsPlugin : SamplePluginBase
    {
        protected override ILogger _logger { get; }
        protected override ITouchPortalClient _client { get; }

        private List<ShortConnectorIdNotificationEvent> _shortIds = new List<ShortConnectorIdNotificationEvent>();

        public ConnectorsPlugin(ITouchPortalClientFactory clientFactory,
                            ILogger<ConnectorsPlugin> logger)
        {
            _logger = logger;
            _client = clientFactory.Create(this);
        }

        public void Run()
        {
            //Connect to Touch Portal:
            _client.Connect();

            _client.ConnectorUpdate("connector.without.data", Random.Shared.Next(0, 100));
            _client.ConnectorUpdate("connector.with.data|first=lower", Random.Shared.Next(0, 100));
            _client.ConnectorUpdate("connector.with.data|first=upper", Random.Shared.Next(0, 100));
        }

        public override void OnConnecterChangeEvent(ConnectorChangeEvent message)
        {
            _logger.LogObjectAsJson(message);

            if(message.ConnectorId == "connector.without.data")
            {
                var value = message.Value / 2;
                _client.ConnectorUpdate("connector.with.data|first=lower", value);

                //TODO: This is really cluncky... need to make simpler shortId lookup. Or track internally.
                var shortId = _shortIds
                    .First(m => m.ConnectorId.Contains("connector.with.data") && m.ConnectorId.Contains("upper"))
                    .ShortId;

                _client.ConnectorUpdate(shortId, value + 50);
                return;
            }

            if (message.ConnectorId == "connector.with.data")
            {
                var first = message.GetValue("first");
                var value = message.Value / 2;
                if (first == "upper")
                    value += 50;
                _client.ConnectorUpdate("connector.without.data", value);
                return;
            }
        }

        public override void OnShortConnectorIdNotificationEvent(ShortConnectorIdNotificationEvent message)
        {
            _logger.LogObjectAsJson(message);
            _shortIds.Add(message);
        }
    }
}
