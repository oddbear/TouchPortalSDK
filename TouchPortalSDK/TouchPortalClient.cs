using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Configuration;
using TouchPortalSDK.Messages.Commands;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Messages.Items;
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

        private InfoEvent _lastInfoEvent;
        private IReadOnlyCollection<Setting> _settings;
        
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
            var pairMessage = new PairCommand(_eventHandler.PluginId);
            var pairJsonMessage = Serialize(pairMessage);
            _touchPortalSocket.SendMessage(pairJsonMessage);

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
        public void Close()
        {
            _touchPortalSocket?.CloseSocket();
            _eventHandler.OnClosedEvent();
        }
        
        private static string Serialize<TMessage>(TMessage message)
            where TMessage : BaseCommand
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            return JsonSerializer.Serialize(message, jsonSerializerOptions);
        }

        private static TMessage Deserialize<TMessage>(string message)
            where TMessage : BaseEvent
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new SettingsConverter()
                }
            };
            return JsonSerializer.Deserialize<TMessage>(message, jsonSerializerOptions);
        }

        #endregion
        
        #region TouchPortal Command Handlers

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool SettingUpdate(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var message = new SettingUpdateCommand(name, value);

            var jsonMessage = Serialize(message);

            var sent = _touchPortalSocket.SendMessage(jsonMessage);

            _logger?.LogInformation($"[{nameof(SettingUpdate)}] '{name}', sent '{sent}'.");

            return true;
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool CreateState(string stateId, string displayName, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(stateId) ||
                string.IsNullOrWhiteSpace(displayName))
                return false;
            
            var message = new CreateStateCommand(stateId, displayName, defaultValue);

            var jsonMessage = Serialize(message);

            var sent = _touchPortalSocket.SendMessage(jsonMessage);

            _logger?.LogInformation($"[{nameof(CreateState)}] '{stateId}', sent '{sent}'.");

            return sent;
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool RemoveState(string stateId)
        {
            if (string.IsNullOrWhiteSpace(stateId))
                return false;

            var message = new RemoveStateCommand(stateId);

            var jsonMessage = Serialize(message);

            var sent = _touchPortalSocket.SendMessage(jsonMessage);

            _logger?.LogInformation($"[{nameof(RemoveState)}] '{stateId}', sent '{sent}'.");

            return sent;
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool StateUpdate(string stateId, string value)
        {
            if (string.IsNullOrWhiteSpace(stateId))
                return false;

            var message = new StateUpdateCommand(stateId, value);

            var jsonMessage = Serialize(message);

            var sent = _touchPortalSocket.SendMessage(jsonMessage);

            _logger?.LogInformation($"[{nameof(StateUpdate)}] '{stateId}', sent '{sent}'.");

            return sent;
        }


        /// <inheritdoc cref="ITouchPortalClient" />
        public bool ChoiceUpdate(string listId, string[] values, string instanceId)
        {
            if (string.IsNullOrWhiteSpace(listId))
                return false;


            var message = new ChoiceUpdateCommand(listId, values, instanceId);

            var jsonMessage = Serialize(message);

            var sent = _touchPortalSocket.SendMessage(jsonMessage);

            _logger?.LogInformation($"[{nameof(ChoiceUpdate)}] '{listId}:{instanceId}', sent '{sent}'.");

            return sent;
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool UpdateActionData(string dataId, double minValue, double maxValue, UpdateActionDataCommand.DataType dataType, string instanceId)
        {
            if (string.IsNullOrWhiteSpace(dataId))
                return false;

            var message = new UpdateActionDataCommand(dataId, minValue, maxValue, dataType, instanceId);

            var jsonMessage = Serialize(message);

            var sent = _touchPortalSocket.SendMessage(jsonMessage);

            _logger?.LogInformation($"[{nameof(ChoiceUpdate)}] '{dataId}:{instanceId}', sent '{sent}'.");

            return sent;
        }

        #endregion

        #region TouchPortal Event Handler

        /// <inheritdoc cref="IJsonEventHandler" />
        public void OnMessage(string jsonMessage)
        {
            if (string.IsNullOrWhiteSpace(jsonMessage))
            {
                Close(); //Socket closed.
                return;
            }

            try
            {
                var messageType = Deserialize<BaseEvent>(jsonMessage)?.Type;
                switch (messageType)
                {
                    case "info":
                        var infoEvent = Deserialize<InfoEvent>(jsonMessage);
                        _lastInfoEvent = infoEvent;
                        _settings = infoEvent.Settings;
                        _infoWaitHandle.Set();

                        _eventHandler.OnInfoEvent(infoEvent);
                        return;
                    case "closePlugin":
                        Deserialize<CloseEvent>(jsonMessage);
                        Close(); //Plugin closed.
                        return;
                    case "listChange":
                        var listChangeEvent = Deserialize<ListChangeEvent>(jsonMessage);
                        _eventHandler.OnListChangedEvent(listChangeEvent);
                        return;
                    case "broadcast":
                        var broadcastEvent = Deserialize<BroadcastEvent>(jsonMessage);
                        _eventHandler.OnBroadcastEvent(broadcastEvent);
                        return;
                    case "settings":
                        var settingsEvent = Deserialize<SettingsEvent>(jsonMessage);
                        _settings = settingsEvent.Values;

                        //TODO: Something better here?
                        _eventHandler.OnSettingsEvent(settingsEvent);
                        return;
                    case "down":
                    case "up":
                    case "action":
                        var actionEvent = Deserialize<ActionEvent>(jsonMessage);
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
