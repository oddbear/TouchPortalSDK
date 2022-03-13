namespace TouchPortalSDK.Values
{
    public class ConnectorShortId
    {
        public string Value { get; }

        public ConnectorShortId(string shortId)
        {
            Value = shortId;
        }

        public override string ToString()
        {
            return Value?.ToString();
        }
    }
}
