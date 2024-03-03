using System;
using TouchPortalSDK.Interfaces;

namespace TouchPortalSDK.Messages.Commands
{
    public class PairCommand : ITouchPortalCommand
    {
        public string Type => "pair";

        public string Id { get; set; }

        public PairCommand()
        {
            // Empty constructor for no validation or calculations.
        }

        public PairCommand(string pluginId)
        {
            if (string.IsNullOrWhiteSpace(pluginId))
                throw new ArgumentNullException(nameof(pluginId));

            Id = pluginId;
        }
    }
}
