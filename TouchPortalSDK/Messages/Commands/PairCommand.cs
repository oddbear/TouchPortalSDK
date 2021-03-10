using System;

namespace TouchPortalSDK.Messages.Commands
{
    public class PairCommand : ITouchPortalCommand
    {
        public string Type => "pair";

        public string Id { get; }

        public PairCommand(string pluginId)
        {
            if (string.IsNullOrWhiteSpace(pluginId))
                throw new ArgumentNullException(nameof(pluginId));

            Id = pluginId;
        }

        public string GetKey()
            => Id;
    }
}
