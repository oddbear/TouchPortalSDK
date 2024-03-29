﻿using Microsoft.Extensions.Logging;
using TouchPortalSDK;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Messages.Models;

namespace SampleUtils
{
    public abstract class SamplePluginBase : ITouchPortalEventHandler
    {
        public string PluginId => PluginInfo.AssemblyName;

        protected abstract ILogger _logger { get; }
        protected abstract ITouchPortalClient _client { get; }

        public virtual void OnActionEvent(ActionEvent message)
        {
            _logger.LogObjectAsJson(message);
        }

        public virtual void OnBroadcastEvent(BroadcastEvent message)
        {
            _logger.LogObjectAsJson(message);
        }

        public virtual void OnClosedEvent(string message)
        {
            _logger.LogObjectAsJson(message);

            //Optional force exits this plugin.
            Environment.Exit(0);
        }

        public virtual void OnConnecterChangeEvent(ConnectorChangeEvent message)
        {
            _logger.LogObjectAsJson(message);
        }

        public virtual void OnInfoEvent(InfoEvent message)
        {
            _logger.LogObjectAsJson(message);
        }

        public virtual void OnListChangedEvent(ListChangeEvent message)
        {
            _logger.LogObjectAsJson(message);
        }

        public virtual void OnNotificationOptionClickedEvent(NotificationOptionClickedEvent message)
        {
            _logger.LogObjectAsJson(message);
        }

        public virtual void OnSettingsEvent(SettingsEvent message)
        {
            _logger.LogObjectAsJson(message);
        }

        public virtual void OnShortConnectorIdNotificationEvent(ConnectorInfo connectorInfo)
        {
            _logger.LogObjectAsJson(connectorInfo);
        }

        public virtual void OnUnhandledEvent(string jsonMessage)
        {
            _logger.LogObjectAsJson(jsonMessage);
        }
    }
}
