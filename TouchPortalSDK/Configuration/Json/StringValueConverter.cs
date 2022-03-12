using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TouchPortalSDK.Configuration.Json
{
    public class StringValueConverter<TType> : JsonConverter<TType>
        where TType : ITypedId, new()
    {
        public override TType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var typedString = new TType();
                typedString.SetValue(reader.GetString());

                return typedString;
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, TType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }

    public interface ITypedId
    {
        string Value { get; }
        void SetValue(string value);
    }
}
