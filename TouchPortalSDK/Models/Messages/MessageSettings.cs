using System.Collections.Generic;
using System.Text.Json.Serialization;
using TouchPortalSDK.Models.Messages.Items;
using TouchPortalSDK.Models.Utils;

namespace TouchPortalSDK.Models.Messages
{
    public class MessageSettings : MessageBase
    {
        /// <summary>
        /// Values in settings.
        /// </summary>
        [JsonConverter(typeof(SettingsConverter))]
        public IReadOnlyCollection<SettingItem> Values { get; set; }
    }
}
