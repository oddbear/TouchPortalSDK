using System;
using TouchPortalSDK.Interfaces;

namespace TouchPortalSDK.Messages.Commands
{
    public class CreateStateCommand : ITouchPortalCommand
    {
        public string Type => "createState";

        public string Id { get; set; }

        public string Desc { get; set; }

        public string DefaultValue { get; set; }

        public string? ParentGroup { get; set; }

        public bool? ForceUpdate { get; set; }

        public static CreateStateCommand CreateAndValidate(string stateId, string desc, string defaultValue, string? parentGroup = null, bool? forceUpdate = null)
        {
            if (string.IsNullOrWhiteSpace(stateId))
                throw new ArgumentNullException(nameof(stateId));

            if (string.IsNullOrWhiteSpace(desc))
                throw new ArgumentNullException(nameof(desc));

            var command = new CreateStateCommand
            {
                Id = stateId,
                Desc = desc,
                DefaultValue = defaultValue ?? string.Empty,
                ParentGroup = parentGroup,
                ForceUpdate = forceUpdate
            };

            return command;
        }
    }
}
