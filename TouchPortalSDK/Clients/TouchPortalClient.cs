using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Configuration;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Commands;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Messages.Models;
using TouchPortalSDK.Messages.Models.Enums;

namespace TouchPortalSDK.Clients
{
    [SuppressMessage("Critical Code Smell", "S1006:Method overrides should not change parameter defaults", Justification = "Service resolved from IoC framework.")]
    public class TouchPortalClient : ITouchPortalClient, IMessageHandler
    {
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
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var command = new SettingUpdateCommand(name, value);

            return SendCommand(command);
        }

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.CreateState(string stateId, string desc, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(stateId) ||
                string.IsNullOrWhiteSpace(desc))
                return false;
            
            var command = new CreateStateCommand(stateId, desc, defaultValue);

            return SendCommand(command);
        }

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.RemoveState(string stateId)
        {
            if (string.IsNullOrWhiteSpace(stateId))
                return false;

            var command = new RemoveStateCommand(stateId);

            return SendCommand(command);
        }

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.StateUpdate(string stateId, string value)
        {
            if (string.IsNullOrWhiteSpace(stateId))
                return false;

            var command = new StateUpdateCommand(stateId, value);

            return SendCommand(command);
        }

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.ChoiceUpdate(string choiceId, string[] values, string instanceId)
        {
            if (string.IsNullOrWhiteSpace(choiceId))
                return false;
            
            var command = new ChoiceUpdateCommand(choiceId, values, instanceId);

            return SendCommand(command);
        }

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.UpdateActionData(string dataId, double minValue, double maxValue, ActionDataType dataType, string instanceId)
        {
            if (string.IsNullOrWhiteSpace(dataId))
                return false;

            var command = new UpdateActionDataCommand(dataId, minValue, maxValue, dataType, instanceId);
            
            return SendCommand(command);
        }

        /// <inheritdoc cref="ICommandHandler" />
        bool ICommandHandler.ShowNotification(string notificationId, string title, string message, NotificationOptions[] notificationOptions)
        {
            if (string.IsNullOrWhiteSpace(notificationId))
                return false;

            if (string.IsNullOrWhiteSpace(title))
                return false;

            if (string.IsNullOrWhiteSpace(message))
                return false;

            if (notificationOptions is null || notificationOptions.Length == 0)
                return false;

            var command = new ShowNotificationCommand(notificationId, title, message, notificationOptions);

            return SendCommand(command);
        }

        public bool SendCommand<TCommand>(TCommand command, [CallerMemberName]string callerMemberName = "")
            where TCommand : ITouchPortalMessage
        {
            var jsonMessage = JsonSerializer.Serialize(command, Options.JsonSerializerOptions);

            var success = _touchPortalSocket.SendMessage(jsonMessage);

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
