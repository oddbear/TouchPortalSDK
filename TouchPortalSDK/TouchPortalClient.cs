using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Configuration;
using TouchPortalSDK.Messages.Commands;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Models;
using TouchPortalSDK.Sockets;
using TouchPortalSDK.Utils;

namespace TouchPortalSDK
{
    [SuppressMessage("Critical Code Smell", "S1006:Method overrides should not change parameter defaults", Justification = "Service resolved from IoC framework.")]
    public class TouchPortalClient : ITouchPortalClient, IJsonEventHandler
    {
        private readonly ILogger<TouchPortalClient> _logger;
        private readonly ITouchPortalEventHandler _eventHandler;
        private readonly ITouchPortalSocket _touchPortalSocket;

        private readonly AutoResetEvent _infoWaitHandle;
        private readonly StateManager _states;

        private InfoEvent _lastInfoEvent;

        /// <inheritdoc cref="ITouchPortalClient" />
        public IStateManager States => _states;

        public TouchPortalClient(ITouchPortalEventHandler eventHandler,
                                 ITouchPortalSocketFactory socketFactory,
                                 ILogger<TouchPortalClient> logger = null)
        {
            if (string.IsNullOrWhiteSpace(eventHandler.PluginId))
                throw new InvalidOperationException($"{nameof(ITouchPortalEventHandler)}: PluginId cannot be null or empty.");

            _eventHandler = eventHandler;
            _touchPortalSocket = socketFactory.Create(this);
            _logger = logger;

            _infoWaitHandle = new AutoResetEvent(false);
            _states = new StateManager();
        }
        
        #region Setup
        
        /// <inheritdoc cref="ITouchPortalClient" />
        public bool Connect()
        {
            //Connect:
            _logger?.LogInformation("Connecting to TouchPortal.");
            var connected = _touchPortalSocket.Connect();
            if (!connected)
                return false;

            //Pair:
            _logger?.LogInformation("Sending pair message.");
            var pairCommand = new PairCommand(_eventHandler.PluginId);
            SendCommand(pairCommand);

            //Listen:
            _logger?.LogInformation("Create listener.");
            var listening = _touchPortalSocket.Listen();
            _logger?.LogInformation("Listener created.");
            if (!listening)
                return false;

            //Waiting for InfoMessage:
            _infoWaitHandle.WaitOne(-1);
            _logger?.LogInformation("Received pair response.");

            return true;
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        public void Close(string message)
        {
            _logger?.LogInformation($"Closing TouchPortal Plugin: '{message}'");

            _touchPortalSocket?.CloseSocket();
            _eventHandler.OnClosedEvent(message);

        }
        
        #endregion
        
        #region TouchPortal Command Handlers

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool SettingUpdate(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var command = new SettingUpdateCommand(name, value);

            return SendCommand(command);
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool CreateState(string stateId, string desc, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(stateId) ||
                string.IsNullOrWhiteSpace(desc))
                return false;
            
            var command = new CreateStateCommand(stateId, desc, defaultValue);

            return SendCommand(command);
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool RemoveState(string stateId)
        {
            if (string.IsNullOrWhiteSpace(stateId))
                return false;

            var command = new RemoveStateCommand(stateId);

            return SendCommand(command);
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool StateUpdate(string stateId, string value)
        {
            if (string.IsNullOrWhiteSpace(stateId))
                return false;

            var command = new StateUpdateCommand(stateId, value);

            return SendCommand(command);
        }
        
        /// <inheritdoc cref="ITouchPortalClient" />
        public bool ChoiceUpdate(string choiceId, string[] values, string instanceId)
        {
            if (string.IsNullOrWhiteSpace(choiceId))
                return false;
            
            var command = new ChoiceUpdateCommand(choiceId, values, instanceId);

            return SendCommand(command);
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool UpdateActionData(string dataId, double minValue, double maxValue, UpdateActionDataCommand.DataType dataType, string instanceId)
        {
            if (string.IsNullOrWhiteSpace(dataId))
                return false;

            var command = new UpdateActionDataCommand(dataId, minValue, maxValue, dataType, instanceId);
            
            return SendCommand(command);
        }

        private bool SendCommand<TCommand>(TCommand command, [CallerMemberName]string callerMember = "")
            where TCommand : ITouchPortalCommand
        {
            var jsonMessage = JsonSerializer.Serialize(command, Options.JsonSerializerOptions);
            var success = _touchPortalSocket.SendMessage(jsonMessage);
            if (success)
                _states.AddStateOfCommand(command);
            
            _logger?.LogInformation($"[{callerMember}] '{command.GetKey()}', sent '{success}'.");

            return success;
        }

        #endregion

        #region TouchPortal Event Handler

        /// <inheritdoc cref="IJsonEventHandler" />
        public void OnMessage(string jsonMessage)
        {
            if (string.IsNullOrWhiteSpace(jsonMessage))
            {
                Close("Socket closed.");
                return;
            }

            try
            {
                var messageType = JsonSerializer.Deserialize<BaseEvent>(jsonMessage, Options.JsonSerializerOptions)?.Type;
                switch (messageType)
                {
                    case "info":
                        var infoEvent = JsonSerializer.Deserialize<InfoEvent>(jsonMessage, Options.JsonSerializerOptions);
                        _lastInfoEvent = infoEvent;
                        _infoWaitHandle.Set();

                        _eventHandler.OnInfoEvent(infoEvent);
                        return;
                    case "closePlugin":
                        JsonSerializer.Deserialize<CloseEvent>(jsonMessage, Options.JsonSerializerOptions);
                        Close("Plugin close event.");
                        return;
                    case "listChange":
                        var listChangeEvent = JsonSerializer.Deserialize<ListChangeEvent>(jsonMessage, Options.JsonSerializerOptions);
                        _eventHandler.OnListChangedEvent(listChangeEvent);
                        return;
                    case "broadcast":
                        var broadcastEvent = JsonSerializer.Deserialize<BroadcastEvent>(jsonMessage, Options.JsonSerializerOptions);
                        _eventHandler.OnBroadcastEvent(broadcastEvent);
                        return;
                    case "settings":
                        var settingsEvent = JsonSerializer.Deserialize<SettingsEvent>(jsonMessage, Options.JsonSerializerOptions);
                        _eventHandler.OnSettingsEvent(settingsEvent);
                        return;
                    case "down":
                    case "up":
                    case "action":
                        var actionEvent = JsonSerializer.Deserialize<ActionEvent>(jsonMessage, Options.JsonSerializerOptions);
                        _eventHandler.OnActionEvent(actionEvent);
                        break;
                    default:
                        _eventHandler.OnUnhandledEvent(jsonMessage);
                        return;
                }
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Could not process message.");
                throw;
            }
        }

        #endregion
    }
}
