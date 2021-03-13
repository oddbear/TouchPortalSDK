using TouchPortalSDK.Models.Enums;

namespace TouchPortalSDK
{
    public interface ICommandHandler
    {
        /// <summary>
        /// Send a custom command. There is no state tracking for this.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        bool SendMessage(string message);

        /// <summary>
        /// Creates a dynamic state in TouchPortal Memory.
        /// This state will disappear when restarting TouchPortal.
        /// You will need to persist them yourself and reload them on plugin load.
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="desc">Description of the created state (name in menus).</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        bool CreateState(string stateId, string desc, string defaultValue = "");

        /// <summary>
        /// Updates a setting in TouchPortal.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool SettingUpdate(string name, string value = "");

        /// <summary>
        /// Removes the dynamic state from TouchPortal.
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        bool RemoveState(string stateId);

        /// <summary>
        /// Value that can be displayed, or an event can trigger on.
        /// Values are not persisted, and will fallback to default value on restart.
        /// - Plugin: Defined in the Entry.tp
        /// - Dynamic: Created or removed at runtime. (in memory only)
        /// - Global: Defined in the TouchPortal UI. (state definition persisted in %AppData%\TouchPortal\states.tp)
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool StateUpdate(string stateId, string value = "");

        /// <summary>
        /// Updates the drop down choices in the TouchPortal UI.
        /// InstanceId can be used to dynamically update a dropdown based on the value chosen from another dropdown.
        /// </summary>
        /// <param name="choiceId">Id of UI dropdown.</param>
        /// <param name="values">Values as string array that you can choose from.</param>
        /// <param name="instanceId">if set (fetched from listChange event), this will only update this particular list.</param>
        /// <returns></returns>
        bool ChoiceUpdate(string choiceId, string[] values, string instanceId = default);

        /// <summary>
        /// Updates the constraints of a data value.
        /// </summary>
        /// <param name="dataId">Id of action the number box.</param>
        /// <param name="minValue">Min value the field can be.</param>
        /// <param name="maxValue">Max value the field can be.</param>
        /// <param name="dataType">Type of the data field.</param>
        /// <param name="instanceId">if set (fetched from listChange event), this will only update this particular list.</param>
        /// <returns></returns>
        bool UpdateActionData(string dataId, double minValue, double maxValue, ActionDataType dataType, string instanceId = default);
    }

    public interface ITouchPortalClient : ICommandHandler
    {
        /// <summary>
        /// Connects, pairs, and listens to the TouchPortal application.
        /// </summary>
        /// <returns>connection success status</returns>
        bool Connect();

        /// <summary>
        /// Closes the connection to TouchPortal and shutdowns the plugin in a safe manner.
        /// </summary>
        void Close();
    }
}