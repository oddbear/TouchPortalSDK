using System;

namespace TouchPortalSDK.Messages.Commands
{
    public class CreateStateCommand : ITouchPortalCommand
    {
        public string Type => "createState";

        public string Id { get; }
        public string Desc { get; }
        public string DefaultValue { get; }

        public CreateStateCommand(string stateId, string desc, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(stateId))
                throw new ArgumentNullException(nameof(stateId));

            if (string.IsNullOrWhiteSpace(desc))
                throw new ArgumentNullException(nameof(desc));

            Id = stateId;
            Desc = desc;
            DefaultValue = defaultValue ?? string.Empty;
        }

        public string GetKey()
            => Id;
    }
}
