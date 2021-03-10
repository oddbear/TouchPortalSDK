namespace TouchPortalSDK.Messages.Commands
{
    public class UpdateActionDataCommand : BaseCommand
    {
        public string InstanceId { get; }

        public DataValue Data { get; }
        
        public UpdateActionDataCommand(string dataId, double minValue, double maxValue, DataType dataType, string instanceId = null)
            : base("updateActionData")
        {
            InstanceId = instanceId;
            Data = new DataValue(dataId, minValue, maxValue, dataType);
        }

        public class DataValue
        {
            public string Id { get; }
            public double MinValue { get; }
            public double MaxValue { get; }
            public DataType Type { get; }

            public DataValue(string dataId, double minValue, double maxValue, DataType dataType)
            {
                Id = dataId;
                MinValue = minValue;
                MaxValue = maxValue;
                Type = dataType;
            }
        }

        /// <summary>
        /// Allowed data types for DataValue
        /// </summary>
        public enum DataType
        {
            Number
        }
    }
}
