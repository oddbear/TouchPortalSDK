namespace TouchPortalSDK.Messages.Models
{
    public class Data
    {
        /// <summary>
        /// The id of the data field as string
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The value of the data field as string
        /// </summary>
        public string Value { get; set; }

        public static Data Create(string id, string value)
        {
            return new Data { Id = id, Value = value };
        }
    }
}