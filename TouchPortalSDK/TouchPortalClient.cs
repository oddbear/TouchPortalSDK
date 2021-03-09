using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Messages.Commands;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Messages.Items;
using TouchPortalSDK.Models;
using TouchPortalSDK.Sockets;

namespace TouchPortalSDK
{
    [SuppressMessage("Critical Code Smell", "S1006:Method overrides should not change parameter defaults", Justification = "Service resolved from IoC framework.")]
    public class TouchPortalClient : ITouchPortalClient
    {
        private readonly ILogger<TouchPortalClient> _logger;
        private readonly ITouchPortalSocket _touchPortalSocket;
        private readonly TouchPortalOptions _options;
        private readonly AutoResetEvent _infoWaitHandle;

        private ITouchPortalPlugin _touchPortalPlugin;
        private InfoEvent _lastInfoEvent;
        private IReadOnlyCollection<Setting> _settings;
        
        public TouchPortalClient(ILogger<TouchPortalClient> logger,
                                 ITouchPortalSocket touchPortalSocket,
                                 TouchPortalOptions options)
        {
            if (string.IsNullOrWhiteSpace(options?.PluginId))
                throw new InvalidOperationException("No plugin id configured in TouchPortalOptions.");

            _logger = logger;
            _touchPortalSocket = touchPortalSocket;
            _options = options;

            _infoWaitHandle = new AutoResetEvent(false);
        }

        #region Setup

        public void RegisterPlugin(ITouchPortalPlugin touchPortalPlugin)
            => _touchPortalPlugin = touchPortalPlugin
                ?? throw new ArgumentNullException(nameof(touchPortalPlugin));

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool Connect()
        {
            if (_touchPortalPlugin is null)
                throw new InvalidOperationException($"No plugin registered. Please register plugin using '{nameof(RegisterPlugin)}' method, or a '{nameof(ITouchPortalPlugin)}' in your IoC container.");

            //Connect:
            _logger?.LogInformation("Connecting to TouchPortal.");
            var connected = _touchPortalSocket.Connect();
            if (!connected)
                return false;

            //Pair:
            _logger?.LogInformation("Sending pair message.");
            var pairMessage = new PairCommand(_options.PluginId);
            var pairJsonMessage = Serialize(pairMessage);
            _touchPortalSocket.SendMessage(pairJsonMessage);

            //Listen:
            _logger?.LogInformation("Create listener.");
            var listening = _touchPortalSocket.Listen(OnMessage);
            _logger?.LogInformation("Listener created.");
            if (!listening)
                return false;

            //Waiting for InfoMessage:
            _infoWaitHandle.WaitOne(-1);
            _logger?.LogInformation("Received pair response.");

            return true;
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        public void Close(string reason)
        {
            _logger?.LogInformation($"[Close] '{reason}'");

            _touchPortalSocket?.CloseSocket();
            _touchPortalPlugin.OnClosed();
        }
        
        private static string Serialize<TMessage>(TMessage message)
            where TMessage : BaseCommand
        {
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, IgnoreNullValues = true };
            return JsonSerializer.Serialize(message, jsonSerializerOptions);
        }

        private static TMessage Deserialize<TMessage>(string message)
            where TMessage : BaseEvent
        {
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
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
        
        private void OnMessage(string jsonMessage)
        {
            if (string.IsNullOrWhiteSpace(jsonMessage))
            {
                Close("Socket closed.");
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

                        _touchPortalPlugin.OnInfo(infoEvent);
                        return;
                    case "closePlugin":
                        Deserialize<CloseEvent>(jsonMessage);
                        Close("Plugin closed.");
                        return;
                    case "listChange":
                        var listChangeEvent = Deserialize<ListChangeEvent>(jsonMessage);
                        _touchPortalPlugin.OnListChanged(listChangeEvent);
                        return;
                    case "broadcast":
                        var broadcastEvent = Deserialize<BroadcastEvent>(jsonMessage);
                        _touchPortalPlugin.OnBroadcast(broadcastEvent);
                        return;
                    case "settings":
                        var settingsEvent = Deserialize<SettingsEvent>(jsonMessage);
                        _settings = settingsEvent.Values;

                        //TODO: Something better here?
                        _touchPortalPlugin.OnSettings(settingsEvent);
                        return;
                    case "down":
                    case "up":
                    case "action":
                        var actionEvent = Deserialize<ActionEvent>(jsonMessage);
                        _touchPortalPlugin.OnAction(actionEvent);
                        break;
                    default:
                        _touchPortalPlugin.OnUnhandled(jsonMessage);
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
