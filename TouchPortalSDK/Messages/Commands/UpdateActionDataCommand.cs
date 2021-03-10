using System;

namespace TouchPortalSDK.Messages.Commands
{
    public class UpdateActionDataCommand : ITouchPortalCommand
    {
        public string Type => "updateActionData";

        public string InstanceId { get; }

        public DataValue Data { get; }

        public UpdateActionDataCommand(string dataId, double minValue, double maxValue, DataType dataType, string instanceId = null)
        {
            if (string.IsNullOrWhiteSpace(dataId))
                throw new ArgumentNullException(nameof(dataId));

            Data = new DataValue(dataId, minValue, maxValue, dataType);

            if (!string.IsNullOrWhiteSpace(instanceId))
                InstanceId = instanceId;
        }

        public class DataValue
        {
            public string Id { get; }
            public double MinValue { get; }
            public double MaxValue { get; }
            public DataType Type { get; }

            public DataValue(string dataId, double minValue, double maxValue, DataType dataType)
            {
                if (string.IsNullOrWhiteSpace(dataId))
                    throw new ArgumentNullException(nameof(dataId));

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

        public string GetKey()
            => InstanceId is null ? Data.Id : $"{Data.Id}:{InstanceId}";
    }
}
