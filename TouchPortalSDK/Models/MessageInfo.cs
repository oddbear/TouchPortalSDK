namespace TouchPortalSDK.Models
{
    public class MessageInfo : MessageBase
    {
        public string Status { get; set; }
        public long SdkVersion { get; set; }
        public string TpVersionString { get; set; }
        public long TpVersionCode { get; set; }
        public long PluginVersion { get; set; }
    }
}
