using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Exceptions;
using TouchPortalSDK.Models;
using TouchPortalSDK.Sockets;

namespace TouchPortalSDK
{
    public class TouchPortalClient : ITouchPortalClient
    {
        private readonly ILogger<TouchPortalClient> _logger;
        private readonly ITouchPortalSocket _touchPortalSocket;

        /// <inheritdoc cref="ITouchPortalClient" />
        public Action<MessageInfo> OnInfo { get; set; }

        /// <inheritdoc cref="ITouchPortalClient" />
        public Action<MessageListChange> OnListChanged { get; set; }

        /// <inheritdoc cref="ITouchPortalClient" />
        public Action<MessageBroadcast> OnBroadcast { get; set; }

        /// <inheritdoc cref="ITouchPortalClient" />
        public Action<MessageSettings> OnSettings { get; set; }

        /// <inheritdoc cref="ITouchPortalClient" />
        public Action<MessageAction> OnAction { get; set; }

        /// <inheritdoc cref="ITouchPortalClient" />
        public Action<Exception> OnClosed { get; set; }

        /// <inheritdoc cref="ITouchPortalClient" />
        public Action<JsonDocument> OnUnhandled { get; set; }

        public TouchPortalClient(ILogger<TouchPortalClient> logger,
                                 ITouchPortalSocket touchPortalSocket)
        {
            _logger = logger;
            _touchPortalSocket = touchPortalSocket;
            _touchPortalSocket.OnMessage = OnMessage;
            _touchPortalSocket.OnClose = Close;
        }

        #region Setup

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool Connect()
        {
            //Connect:
            var connected = _touchPortalSocket.Connect();
            if (!connected)
                return false;

            //Pair:
            var json = _touchPortalSocket.Pair();
            if (string.IsNullOrWhiteSpace(json))
                return false;

            var infoMessage = Deserialize<MessageInfo>(json);
            OnInfo?.Invoke(infoMessage);

            //Listen:
            var listening = _touchPortalSocket.Listen();
            if (!listening)
                return false;

            return true;
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        public void Close(Exception exception = default)
        {
            _logger?.LogInformation(exception, "Closing");
            
            _touchPortalSocket?.CloseSocket();
            
            OnClosed?.Invoke(exception);
        }

        #endregion

        #region TouchPortal Input

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool CreateState(string stateId, string displayName, string defaultValue = "")
        {
            if (string.IsNullOrWhiteSpace(stateId) ||
                string.IsNullOrWhiteSpace(displayName))
                return false;

            var message = new Dictionary<string, object>
            {
                ["type"] = "createState",
                ["id"] = stateId,
                ["desc"] = displayName,
                ["defaultValue"] = defaultValue
            };

            var sent = _touchPortalSocket.SendMessage(message);

            _logger?.LogInformation($"Create State '{stateId}', sent '{sent}'.");

            return sent;
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool RemoveState(string stateId)
        {
            if (string.IsNullOrWhiteSpace(stateId))
                return false;

            var message = new Dictionary<string, object>
            {
                ["type"] = "removeState",
                ["id"] = stateId,
            };

            var sent = _touchPortalSocket.SendMessage(message);

            _logger?.LogInformation($"Remove State '{stateId}', sent '{sent}'.");

            return sent;
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool UpdateState(string stateId, string value)
        {
            if (string.IsNullOrWhiteSpace(stateId))
                return false;

            var message = new Dictionary<string, object>
            {
                ["type"] = "stateUpdate",
                ["id"] = stateId,
                ["value"] = value ?? string.Empty
            };

            var sent = _touchPortalSocket.SendMessage(message);

            _logger?.LogInformation($"Update State '{stateId}', sent '{sent}'.");

            return sent;
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        public bool UpdateChoice(string listId, string[] values, string instanceId = null)
        {
            if (string.IsNullOrWhiteSpace(listId))
                return false;

            var message = new Dictionary<string, object>
            {
                ["type"] = "choiceUpdate",
                ["id"] = listId,
                ["value"] = values ?? Array.Empty<string>()
            };

            if (!string.IsNullOrWhiteSpace(instanceId))
                message["instanceId"] = instanceId;

            var sent = _touchPortalSocket.SendMessage(message);

            _logger?.LogInformation($"Update Choice '{listId}', sent '{sent}'.");

            return sent;
        }

        #endregion

        #region TouchPortal Output

        /// <inheritdoc cref="ITouchPortalClient" />
        private static TMessage Deserialize<TMessage>(string message)
            where TMessage : MessageBase
        {
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return JsonSerializer.Deserialize<TMessage>(message, jsonSerializerOptions);
        }

        /// <inheritdoc cref="ITouchPortalClient" />
        private void OnMessage(string json)
        {
            var messageType = Deserialize<MessageBase>(json)?.Type;
            switch (messageType)
            {
                case "closePlugin":
                    var closeMessage = Deserialize<MessageClose>(json);
                    Close(new TouchPortalClosedException(closeMessage));
                    break;
                case "listChange":
                    var listChangeMessage = Deserialize<MessageListChange>(json);
                    OnListChanged?.Invoke(listChangeMessage);
                    break;
                case "broadcast":
                    var broadcastMessage = Deserialize<MessageBroadcast>(json);
                    OnBroadcast?.Invoke(broadcastMessage);
                    break;
                case "settings":
                    var settingsMessage = Deserialize<MessageSettings>(json);
                    OnSettings?.Invoke(settingsMessage);
                    break;
                case "down": //TODO: 2.3
                case "up": //TODO: 2.3
                case "action":
                    var actionMessage = Deserialize<MessageAction>(json);
                    OnAction?.Invoke(actionMessage);
                    break;
                default:
                    var jsonDocument = JsonSerializer.Deserialize<JsonDocument>(json);
                    OnUnhandled?.Invoke(jsonDocument);
                    break;
            }
        }

        #endregion
    }
}
