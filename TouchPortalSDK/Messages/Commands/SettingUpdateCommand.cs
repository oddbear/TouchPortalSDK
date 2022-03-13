using System;
using TouchPortalSDK.Interfaces;

namespace TouchPortalSDK.Messages.Commands
{
    public class SettingUpdateCommand : ITouchPortalCommand
    {
        public string Type => "settingUpdate";

        public string Name { get; set; }

        public string Value { get; set; }

        public static SettingUpdateCommand CreateAndValidate(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var command = new SettingUpdateCommand
            {
                Name = name,
                Value = value ?? string.Empty
            };

            return command;
        }
    }
}
