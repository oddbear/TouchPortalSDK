using System;
using Microsoft.Extensions.Logging;
using TouchPortalSDK;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Messages.Models;

namespace SampleUtils
{
    public abstract class SamplePluginBase : ITouchPortalEventHandler
    {
        public string PluginId => PluginInfo.AssemblyName;

        protected abstract ILogger Logger { get; }
        protected abstract ITouchPortalClient Client { get; }

        public virtual void OnActionEvent(ActionEvent message)
        {
            Logger.LogObjectAsJson(message);
        }

        public virtual void OnBroadcastEvent(BroadcastEvent message)
        {
            Logger.LogObjectAsJson(message);
        }

        public virtual void OnClosedEvent(string message)
        {
            Logger.LogObjectAsJson(message);

            // Optional force exits this plugin.
            Environment.Exit(0);
        }

        public virtual void OnConnecterChangeEvent(ConnectorChangeEvent message)
        {
            Logger.LogObjectAsJson(message);
        }

        public virtual void OnInfoEvent(InfoEvent message)
        {
            Logger.LogObjectAsJson(message);
        }

        public virtual void OnListChangedEvent(ListChangeEvent message)
        {
            Logger.LogObjectAsJson(message);
        }

        public virtual void OnNotificationOptionClickedEvent(NotificationOptionClickedEvent message)
        {
            Logger.LogObjectAsJson(message);
        }

        public virtual void OnSettingsEvent(SettingsEvent message)
        {
            Logger.LogObjectAsJson(message);
        }

        public virtual void OnShortConnectorIdNotificationEvent(ConnectorInfo connectorInfo)
        {
            Logger.LogObjectAsJson(connectorInfo);
        }

        public virtual void OnUnhandledEvent(string jsonMessage)
        {
            Logger.LogObjectAsJson(jsonMessage);
        }
    }
}
