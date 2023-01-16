using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using TouchPortalSDK.Configuration;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Commands;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Messages.Models;
using TouchPortalSDK.Messages.Models.Enums;
using TouchPortalSDK.Values;

namespace TouchPortalSDK.Clients
{
    [SuppressMessage("Critical Code Smell", "S1006:Method overrides should not change parameter defaults", Justification = "Service resolved from IoC framework.")]
    public class TouchPortalClient : ITouchPortalClient, IMessageHandler, ICommandHandler
    {
        /// <inheritdoc cref="ITouchPortalClient" />
        public bool IsConnected => _touchPortalSocket?.IsConnected ?? false;

        private readonly ILogger<TouchPortalClient> _logger;
        private readonly ITouchPortalEventHandler _eventHandler;
        private readonly ITouchPortalSocket _touchPortalSocket;

        private readonly ManualResetEvent _infoWaitHandle;

        private InfoEvent _lastInfoEvent;

        public TouchPortalClient(ITouchPortalEventHandler eventHandler,
                                 ITouchPortalSocketFactory socketFactory,
                                 ILoggerFactory loggerFactory = null)
        {
            if (string.IsNullOrWhiteSpace(eventHandler?.PluginId))
                throw new InvalidOperationException($"{nameof(ITouchPortalEventHandler)}: PluginId cannot be null or empty.");

            _eventHandler = eventHandler;
            _touchPortalSocket = socketFactory.Create(this);
            _logger = loggerFactory?.CreateLogger<TouchPortalClient>();

            _infoWaitHandle = new ManualResetEvent(false);
        }

        #region Setup

        /// <inheritdoc cref="ITouchPortalClient" />
        bool ITouchPortalClient.Connect()
        {
            //Connect:
            _logger?.LogInformation("Connecting to TouchPortal.");
            var connected = _touchPortalSocket.Connect();
            if (!connected)
                return false;

            //Pair:
            _logger?.LogInformation("Sending pair message.");
            var pairCommand = new PairCommand(_eventHandler.PluginId);
            var pairing = SendCommand(pairCommand);
            if (!pairing)
                return false;

            //Listen:
            _logger?.LogInformation("Create listener.");
            var listening = _touchPortalSocket.Listen();
            _logger?.LogInformation("Listener created.");
            if (!listening)
                return false;

            //Waiting for InfoMessage:
            _infoWaitHandle.WaitOne(-1);
            _logger?.LogInformation("Received pair response.");

            return _lastInfoEvent != null;
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        void ITouchPortalClient.Close()
            => Close("Closed by plugin.");

        /// <inheritdoc cref="IMessageHandler" />
        public void Close(string message, Exception exception = default)
        {
            _logger?.LogInformation(exception, $"Closing TouchPortal Plugin: '{message}'");

            _touchPortalSocket?.CloseSocket();

            _eventHandler.OnClosedEvent(message);
        }

        #endregion

        #region TouchPortal Command Handlers

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.SettingUpdate(string name, string value)
        {
            try
            {
                var command = SettingUpdateCommand.CreateAndValidate(name, value);

                return SendCommand(command);
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, $"Failed to create command {nameof(SettingUpdateCommand)}");
                return false;
            }
        }

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.CreateState(string stateId, string desc, string defaultValue, string parentGroup)
        {
            try
            {
                var command = CreateStateCommand.CreateAndValidate(stateId, desc, defaultValue, parentGroup);

                return SendCommand(command);
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, $"Failed to create command {nameof(CreateStateCommand)}");
                return false;
            }
        }

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.RemoveState(string stateId)
        {
            try
            {
                var command = RemoveStateCommand.CreateAndValidate(stateId);

                return SendCommand(command);
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, $"Failed to create command {nameof(RemoveStateCommand)}");
                return false;
            }
        }

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.StateUpdate(string stateId, string value)
        {
            try
            {
                var command = StateUpdateCommand.CreateAndValidate(stateId, value);

                return SendCommand(command);
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, $"Failed to create command {nameof(StateUpdateCommand)}");
                return false;
            }
        }

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.ChoiceUpdate(string choiceId, string[] values, string instanceId)
        {
            try
            {
                var command = ChoiceUpdateCommand.CreateAndValidate(choiceId, values, instanceId);

                return SendCommand(command);
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, $"Failed to create command {nameof(ChoiceUpdateCommand)}");
                return false;
            }
        }

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.UpdateActionData(string dataId, double minValue, double maxValue, ActionDataType dataType, string instanceId)
        {
            try
            {
                var command = UpdateActionDataCommand.CreateAndValidate(dataId, minValue, maxValue, dataType, instanceId);

                return SendCommand(command);
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, $"Failed to create command {nameof(UpdateActionDataCommand)}");
                return false;
            }
        }

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.ShowNotification(string notificationId, string title, string message, NotificationOptions[] notificationOptions)
        {
            try
            {
                var command = ShowNotificationCommand.CreateAndValidate(notificationId, title, message, notificationOptions);

                return SendCommand(command);
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, $"Failed to create command {nameof(ShowNotificationCommand)}");
                return false;
            }
        }

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.ConnectorUpdate(string connectorId, int value)
        {
            try
            {
                var command = ConnectorUpdateCommand.CreateAndValidate(_eventHandler.PluginId, connectorId, value);

                return SendCommand(command);
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, $"Failed to create command {nameof(ConnectorUpdateCommand)}");
                return false;
            }
        }

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.ConnectorUpdate(ConnectorShortId shortId, int value)
        {
            try
            {
                var command = ConnectorUpdateCommand.CreateAndValidate(shortId, value);

                return SendCommand(command);
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, $"Failed to create command {nameof(ConnectorUpdateCommand)}");
                return false;
            }
        }

        bool ICommandHandler.SendCommand(ITouchPortalCommand command)
        {
            var jsonMessage = JsonSerializer.Serialize<object>(command, Options.JsonSerializerOptions);

            return _touchPortalSocket.SendMessage(jsonMessage);
        }

        public bool SendCommand(ITouchPortalCommand command, [CallerMemberName] string callerMemberName = "")
        {
            var success = ((ICommandHandler)this).SendCommand(command);
            _logger?.LogInformation($"[{callerMemberName}] sent: '{success}'.");
            return success;
        }

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.SendMessage(string message)
            => _touchPortalSocket.SendMessage(message);

        #endregion

        #region TouchPortal Event Handler

        /// <inheritdoc cref="IMessageHandler" />
        void IMessageHandler.OnMessage(string message)
        {
            var eventMessage = MessageResolver.ResolveMessage(message);
            switch (eventMessage)
            {
                case InfoEvent infoEvent:
                    _lastInfoEvent = infoEvent;
                    _infoWaitHandle.Set();

                    _eventHandler.OnInfoEvent(infoEvent);
                    return;
                case CloseEvent _:
                    Close("TouchPortal sent a Plugin close event.");
                    return;
                case ListChangeEvent listChangeEvent:
                    _eventHandler.OnListChangedEvent(listChangeEvent);
                    return;
                case BroadcastEvent broadcastEvent:
                    _eventHandler.OnBroadcastEvent(broadcastEvent);
                    return;
                case SettingsEvent settingsEvent:
                    _eventHandler.OnSettingsEvent(settingsEvent);
                    return;
                case NotificationOptionClickedEvent notificationEvent:
                    _eventHandler.OnNotificationOptionClickedEvent(notificationEvent);
                    return;
                case ConnectorChangeEvent connectorChangeEvent:
                    _eventHandler.OnConnecterChangeEvent(connectorChangeEvent);
                    return;
                case ShortConnectorIdNotificationEvent shortConnectorIdEvent:
                    _eventHandler.OnShortConnectorIdNotificationEvent(new ConnectorInfo(shortConnectorIdEvent));
                    return;
                //All of Action, Up, Down:
                case ActionEvent actionEvent:
                    _eventHandler.OnActionEvent(actionEvent);
                    return;
                default:
                    _eventHandler.OnUnhandledEvent(message);
                    return;
            }
        }

        #endregion
    }
}
