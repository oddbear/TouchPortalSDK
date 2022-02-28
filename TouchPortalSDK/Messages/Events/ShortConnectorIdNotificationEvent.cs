using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models;

namespace TouchPortalSDK.Messages.Events
{
    public class ShortConnectorIdNotificationEvent : ITouchPortalMessage
    {
        public string Type { get; set; }
        public string PluginId { get; set; }
        public string ConnectorId { get; set; }
        public ConnectorShortId ShortId { get; set; }

        public Identifier GetIdentifier()
            => new Identifier(Type, ConnectorId, default);
    }

    public class ConnectorShortId : ITypedId
    {
        public string Value { get; }

        public ConnectorShortId(string shortId)
        {
            Value = shortId;
        }

        public override string ToString()
        {
            return Value?.ToString();
        }
    }

    public interface ITypedId
    {
        string Value { get; }
    }

    public class StringConverter<TType> : JsonConverter<TType>
        where TType : ITypedId
    {
        public override TType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return (TType)Activator.CreateInstance(typeToConvert, reader.GetString());
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, TType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }

    }
}
