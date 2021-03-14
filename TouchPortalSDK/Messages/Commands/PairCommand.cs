using System;
using TouchPortalSDK.Messages.Items;

namespace TouchPortalSDK.Messages.Commands
{
    public class PairCommand : ITouchPortalMessage
    {
        public string Type => "pair";

        public string Id { get; set; }

        public PairCommand(string pluginId)
        {
            if (string.IsNullOrWhiteSpace(pluginId))
                throw new ArgumentNullException(nameof(pluginId));

            Id = pluginId;
        }

        /// <inheritdoc cref="ITouchPortalMessage" />
        public Identifier GetIdentifier()
            => new Identifier(Type, Id, default);
    }
}
