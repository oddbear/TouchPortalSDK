using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using TouchPortalSDK.Models.Messages.Items;

namespace TouchPortalSDK.Models.Utils
{
    public class SettingsConverter : JsonConverter<IReadOnlyCollection<SettingItem>>
    {
        public override IReadOnlyCollection<SettingItem> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var jsonDocument = JsonDocument.ParseValue(ref reader))
            {
                return jsonDocument.RootElement.EnumerateArray()
                    //TODO: This might change in final version.
                    //The object has only one property.
                    //Format is { "Some name" : "some value" }
                    //It's not like the input of { "name":"Some Name", "value":"Some value" }
                    .Select(jsonElement => jsonElement.EnumerateObject().Single())
                    .Select(jsonProperty => new SettingItem
                    {
                        Name = jsonProperty.Name,
                        Value = jsonProperty.Value.GetString()
                    })
                    .ToArray();
            }
        }

        public override void Write(Utf8JsonWriter writer, IReadOnlyCollection<SettingItem> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}