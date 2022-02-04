﻿using System;
using System.Linq;
using System.Text;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models;

namespace TouchPortalSDK.Messages.Commands
{
    public class ConnectorUpdateCommand : ITouchPortalMessage
    {
        public string Type => "connectorUpdate";

        public string ConnectorId { get; set; }

        public int Value { get; set; }

        public ConnectorUpdateCommand(string pluginId, string connectorId, int value)
        {
            if (string.IsNullOrWhiteSpace(pluginId))
                throw new ArgumentNullException(nameof(pluginId));

            if (string.IsNullOrWhiteSpace(connectorId))
                throw new ArgumentNullException(nameof(connectorId));

            if (value < 0 || value > 100)
                throw new ArgumentException("Value must be between 0 and 100", nameof(value));

            ConnectorId = $"pc_{pluginId}_{connectorId}";
            Value = value;
        }

        public Identifier GetIdentifier()
            => new Identifier(Type, ConnectorId, default);
    }
}