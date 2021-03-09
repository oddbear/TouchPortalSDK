using System.Collections.Generic;
using System.Text.Json.Serialization;
using TouchPortalSDK.Messages.Items;
using TouchPortalSDK.Utils;

namespace TouchPortalSDK.Messages.Events
{
    public class SettingsEvent : BaseEvent
    {
        /// <summary>
        /// Values in settings.
        /// </summary>
        [JsonConverter(typeof(SettingsConverter))]
        public IReadOnlyCollection<Setting> Values { get; set; }
    }
}
