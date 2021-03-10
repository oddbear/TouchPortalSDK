namespace TouchPortalSDK.Messages.States
{
    public class Choice
    {
        public string Id { get; }

        public string[] Value { get; }

        public string InstanceId { get; }

        public Choice(string id, string[] value, string instanceId)
        {
            Id = id;
            Value = value;
            InstanceId = instanceId;
        }
    }
}
