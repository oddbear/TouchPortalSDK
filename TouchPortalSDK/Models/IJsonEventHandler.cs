namespace TouchPortalSDK.Models
{
    public interface IJsonEventHandler
    {
        /// <summary>
        /// Method for handling serialized json message events.
        /// </summary>
        /// <param name="jsonMessage"></param>
        void OnMessage(string jsonMessage);
    }
}