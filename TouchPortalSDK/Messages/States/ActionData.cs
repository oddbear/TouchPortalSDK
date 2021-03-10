using TouchPortalSDK.Messages.Commands;

namespace TouchPortalSDK.Messages.States
{
    public class ActionData
    {
        public string Id { get; }
        public double MinValue { get; }
        public double MaxValue { get; }
        public UpdateActionDataCommand.DataType Type { get; }

        public ActionData(string id, double minValue, double maxValue, UpdateActionDataCommand.DataType type)
        {
            Id = id;
            MinValue = minValue;
            MaxValue = maxValue;
            Type = type;
        }
    }
}
