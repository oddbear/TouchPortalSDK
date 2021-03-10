using System;

namespace TouchPortalSDK.Messages.Commands
{
    public class SettingUpdateCommand : ITouchPortalCommand
    {
        public string Type => "settingUpdate";

        public string Name { get; }
        public string Value { get; }

        public SettingUpdateCommand(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Value = value ?? string.Empty;
        }
    }
}
