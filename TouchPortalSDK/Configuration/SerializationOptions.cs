using System.Text.Json;
using System.Text.Json.Serialization;

namespace TouchPortalSDK.Configuration
{
    public static class Options
    {
        public static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new SettingsConverter()
            }
        };
    }
}
