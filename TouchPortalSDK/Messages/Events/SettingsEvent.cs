using System.Collections.Generic;
using TouchPortalSDK.Messages.Items;

namespace TouchPortalSDK.Messages.Events
{
    public class SettingsEvent : ITouchPortalMessage
    {
        /// <summary>
        /// Plugin settings changed in TouchPortal UI.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Values in settings.
        /// </summary>
        public IReadOnlyCollection<Setting> Values { get; set; }

        public Identifier GetIdentifier()
            => new Identifier(Type, default, default);
    }
}
