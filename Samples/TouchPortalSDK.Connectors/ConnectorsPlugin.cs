using Microsoft.Extensions.Logging;
using SampleUtils;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Messages.Models;

namespace TouchPortalSDK.Notifications
{
    public class ConnectorsPlugin : SamplePluginBase
    {
        protected override ILogger Logger { get; }
        protected override ITouchPortalClient Client { get; }

        private readonly List<ConnectorInfo> _connectors = [];

        public ConnectorsPlugin(
            ITouchPortalClientFactory clientFactory,
            ILogger<ConnectorsPlugin> logger)
        {
            Logger = logger;
            Client = clientFactory.Create(this);
        }

        public void Run()
        {
            // Connect to Touch Portal:
            Client.Connect();

            Client.ConnectorUpdate("connector.without.data", Random.Shared.Next(0, 100));

            // Connectors with data is a littble bit cluncy, you will need to add the parameters in this format.
            // Warning: The parameters have to be in the correct order!
            Client.ConnectorUpdate("connector.with.data|first=lower", Random.Shared.Next(0, 100));
            Client.ConnectorUpdate("connector.with.data|first=upper", Random.Shared.Next(0, 100));
        }

        public override void OnConnecterChangeEvent(ConnectorChangeEvent message)
        {
            Logger.LogObjectAsJson(message);

            if (message.ConnectorId == "connector.without.data")
            {
                var value = message.Value / 2;
                Client.ConnectorUpdate("connector.with.data|first=lower", value);

                // You can also track the connectors that are set, and get the 
                var connectors = _connectors
                    .Where(connectorInfo => connectorInfo.ConnectorId == "connector.with.data")
                    // You can write filters to get one or more connectors, then update them accordingly.
                    .Where(connectorInfo => connectorInfo.GetValue("first") == "upper");

                foreach (var connector in connectors)
                {
                    Client.ConnectorUpdate(connector.ShortId, value + 50);
                }

                return;
            }

            if (message.ConnectorId == "connector.with.data")
            {
                var first = message.GetValue("first");
                var value = message.Value / 2;
                if (first == "upper")
                    value += 50;

                Client.ConnectorUpdate("connector.without.data", value);

                return;
            }
        }

        public override void OnShortConnectorIdNotificationEvent(ConnectorInfo connectorInfo)
        {
            Logger.LogObjectAsJson(connectorInfo);

            _connectors.Add(connectorInfo);
        }
    }
}
