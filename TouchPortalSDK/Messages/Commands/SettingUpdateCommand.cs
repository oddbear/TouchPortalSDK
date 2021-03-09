namespace TouchPortalSDK.Messages.Commands
{
    public class SettingUpdateCommand : BaseCommand
    {
        public string Name { get; }
        public string Value { get; }

        public SettingUpdateCommand(string name, string value)
            : base("settingUpdate")
        {
            Name = name;
            Value = value ?? string.Empty;
        }
    }
}
