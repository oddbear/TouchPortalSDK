using System;
using System.Text.Json.Serialization;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Values;

namespace TouchPortalSDK.Messages.Commands
{
    public class ConnectorUpdateCommand : ITouchPortalCommand
    {
        public string Type => "connectorUpdate";

        [JsonPropertyName("connectorId")]
        public string TouchPortalConnectorId { get; set; }

        public string ShortId { get; set; }

        public int Value { get; set; }

        public static ConnectorUpdateCommand CreateAndValidate(string pluginId, string connectorId, int value)
        {
            if (string.IsNullOrWhiteSpace(pluginId))
                throw new ArgumentNullException(nameof(pluginId));

            if (string.IsNullOrWhiteSpace(connectorId))
                throw new ArgumentNullException(nameof(connectorId));

            if (value < 0 || value > 100)
                throw new ArgumentException("Value must be between 0 and 100", nameof(value));

            var command = new ConnectorUpdateCommand
            {
                TouchPortalConnectorId = $"pc_{pluginId}_{connectorId}"
            };

            if (command.TouchPortalConnectorId.Length > 200)
                throw new ArgumentException("ConnectorId longer than 200, use ShortId", nameof(value));

            command.Value = value;

            return command;
        }

        public static ConnectorUpdateCommand CreateAndValidate(ConnectorShortId shortId, int value)
        {
            if (shortId is null)
                throw new ArgumentNullException(nameof(shortId));

            if (string.IsNullOrWhiteSpace(shortId.Value))
                throw new InvalidOperationException("ShortId value was empty.");

            if (value < 0 || value > 100)
                throw new ArgumentException("Value must be between 0 and 100", nameof(value));

            var command = new ConnectorUpdateCommand
            {
                ShortId = shortId.Value,
                Value = value
            };

            return command;
        }
    }
}
