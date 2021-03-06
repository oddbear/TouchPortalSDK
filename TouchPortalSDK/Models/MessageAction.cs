using System.Collections.Generic;
using System.Linq;

namespace TouchPortalSDK.Models
{
    public class MessageAction : MessageBase
    {
        public string PluginId { get; set; }

        /// <summary>
        /// The id of the action.
        /// </summary>
        public string ActionId { get; set; }

        /// <summary>
        /// Data equals selected values of each dropdown.
        /// Ex. data1: dropdown1
        ///     data2: dropdown2
        /// </summary>
        public List<DataItem> Data { get; set; }
        
        public class DataItem
        {
            public string Id { get; set; }
            public string Value { get; set; }
        }

        /// <summary>
        /// Returns the value of the selected item in an action data field.
        /// This value can be null in some cases, and will be null if data field is miss written.
        /// </summary>
        /// <param name="dataId">the id of the datafield.</param>
        /// <returns>the value of the data field as string</returns>
        public string GetValue(string dataId)
            => Data?.SingleOrDefault(data => data.Id == dataId)?.Value;
    }
}
