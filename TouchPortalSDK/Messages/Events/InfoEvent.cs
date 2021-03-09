using System.Collections.Generic;
using System.Text.Json.Serialization;
using TouchPortalSDK.Messages.Items;
using TouchPortalSDK.Utils;

namespace TouchPortalSDK.Messages.Events
{
    public class InfoEvent : BaseEvent
    {
        /// <summary>
        /// Status ex. "paired"
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Version of the SDK this version of TouchPortal knows about.
        /// Ex. 2
        /// </summary>
        public int SdkVersion { get; set; }

        /// <summary>
        /// TouchPortal version as string.
        /// Major, Minor, Patch: M.m.ppp
        /// </summary>
        public string TpVersionString { get; set; }

        /// <summary>
        /// TouchPortal version as int.
        /// Format: Major * 10000 + Minor * 1000 + patch.
        /// </summary>
        public int TpVersionCode { get; set; }

        /// <summary>
        /// TouchPortal version as code.
        /// </summary>
        public int PluginVersion { get; set; }

        /// <summary>
        /// Values in settings.
        /// </summary>
        [JsonConverter(typeof(SettingsConverter))]
        public IReadOnlyCollection<Setting> Settings { get; set; }
    }
}
