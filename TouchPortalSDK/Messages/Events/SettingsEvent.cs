using System.Collections.Generic;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models;

namespace TouchPortalSDK.Messages.Events
{
    public class SettingsEvent : ITouchPortalEvent
    {
        /// <summary>
        /// Plugin settings changed in TouchPortal UI.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Values in settings.
        /// </summary>
        public IReadOnlyCollection<Setting> Values { get; set; }
    }
}
