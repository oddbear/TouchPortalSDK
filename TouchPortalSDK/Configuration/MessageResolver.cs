using System.Text.Json;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Commands;
using TouchPortalSDK.Messages.Events;

namespace TouchPortalSDK.Configuration
{
    internal static class MessageResolver
    {
        internal static ITouchPortalMessage ResolveMessage(string message)
        {
            //Is the message a valid object?
            var jsonDocument = GenericDocument(message);
            if (jsonDocument.ValueKind != JsonValueKind.Object ||
                !jsonDocument.TryGetProperty("type", out var typeElement))
                return null;

            var type = typeElement.GetString();
            switch (type)
            {
                //Events:
                case "info":
                    return JsonSerializer.Deserialize<InfoEvent>(message, Options.JsonSerializerOptions);
                case "closePlugin":
                    return JsonSerializer.Deserialize<CloseEvent>(message, Options.JsonSerializerOptions);
                case "listChange":
                    return JsonSerializer.Deserialize<ListChangeEvent>(message, Options.JsonSerializerOptions);
                case "broadcast":
                    return JsonSerializer.Deserialize<BroadcastEvent>(message, Options.JsonSerializerOptions);
                case "settings":
                    return JsonSerializer.Deserialize<SettingsEvent>(message, Options.JsonSerializerOptions);
                case "down":
                case "up":
                case "action":
                    return JsonSerializer.Deserialize<ActionEvent>(message, Options.JsonSerializerOptions);
                case "notificationOptionClicked":
                    return JsonSerializer.Deserialize<NotificationOptionClickedEvent>(message, Options.JsonSerializerOptions);
                case "connectorChange":
                    return JsonSerializer.Deserialize<ConnectorChangeEvent>(message, Options.JsonSerializerOptions);

                //Commands:
                case "choiceUpdate":
                    return JsonSerializer.Deserialize<ChoiceUpdateCommand>(message, Options.JsonSerializerOptions);
                case "createState":
                    return JsonSerializer.Deserialize<CreateStateCommand>(message, Options.JsonSerializerOptions);
                case "pair":
                    return JsonSerializer.Deserialize<PairCommand>(message, Options.JsonSerializerOptions);
                case "removeState":
                    return JsonSerializer.Deserialize<RemoveStateCommand>(message, Options.JsonSerializerOptions);
                case "settingUpdate":
                    return JsonSerializer.Deserialize<SettingUpdateCommand>(message, Options.JsonSerializerOptions);
                case "stateUpdate":
                    return JsonSerializer.Deserialize<StateUpdateCommand>(message, Options.JsonSerializerOptions);
                case "updateActionData":
                    return JsonSerializer.Deserialize<UpdateActionDataCommand>(message, Options.JsonSerializerOptions);

                default:
                    return null;
            }
        }

        private static JsonElement GenericDocument(string message)
        {
            try
            {
                return JsonSerializer.Deserialize<JsonElement>(message);
            }
            catch
            {
                return new JsonElement();
            }
        }
    }
}
