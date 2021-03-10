namespace TouchPortalSDK.Messages.States
{
    public class State
    {
        public string Id { get; }
        public string Value { get; }

        public State(string stateId, string value)
        {
            Id = stateId;
            Value = value;
        }
    }

    public class StateCreate : State
    {
        public string Desc { get; }

        public StateCreate(string stateId, string desc, string defaultValue)
            : base(stateId, defaultValue)
        {
            Desc = desc;
        }
    }
}
