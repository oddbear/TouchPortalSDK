using System.Collections.Generic;
using TouchPortalSDK.Messages.Items;

namespace TouchPortalSDK.Messages.Events
{
    public class SettingsEvent : BaseEvent
    {
        /// <summary>
        /// Values in settings.
        /// </summary>
        public IReadOnlyCollection<Setting> Values { get; set; }
    }
}
