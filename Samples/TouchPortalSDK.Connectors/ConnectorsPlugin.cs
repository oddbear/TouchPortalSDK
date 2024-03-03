using Microsoft.Extensions.Logging;
using SampleUtils;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Messages.Models;

namespace TouchPortalSDK.Notifications
{
    public class ConnectorsPlugin : SamplePluginBase
    {
        protected override ILogger _logger { get; }
        protected override ITouchPortalClient _client { get; }

        private readonly List<ConnectorInfo> _connectors = new();

        public ConnectorsPlugin(
            ITouchPortalClientFactory clientFactory,
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

            //Connectors with data is a littble bit cluncy, you will need to add the parameters in this format.
            //Warning: The parameters have to be in the correct order!
            _client.ConnectorUpdate("connector.with.data|first=lower", Random.Shared.Next(0, 100));
            _client.ConnectorUpdate("connector.with.data|first=upper", Random.Shared.Next(0, 100));
        }

        public override void OnConnecterChangeEvent(ConnectorChangeEvent message)
        {
            _logger.LogObjectAsJson(message);

            if (message.ConnectorId == "connector.without.data")
            {
                var value = message.Value / 2;
                _client.ConnectorUpdate("connector.with.data|first=lower", value);

                //You can also track the connectors that are set, and get the 
                var connectors = _connectors
                    .Where(connectorInfo => connectorInfo.ConnectorId == "connector.with.data")
                    //You can write filters to get one or more connectors, then update them accordingly.
                    .Where(connectorInfo => connectorInfo.GetValue("first") == "upper");

                foreach (var connector in connectors)
                {
                    _client.ConnectorUpdate(connector.ShortId, value + 50);
                }

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

        public override void OnShortConnectorIdNotificationEvent(ConnectorInfo connectorInfo)
        {
            _logger.LogObjectAsJson(connectorInfo);

            _connectors.Add(connectorInfo);
        }
    }
}
