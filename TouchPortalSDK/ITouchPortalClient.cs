using System;
using System.Text.Json;
using System.Threading.Tasks;
using TouchPortalSDK.Models.Enums;
using TouchPortalSDK.Models.Messages;

namespace TouchPortalSDK
{
    public interface ITouchPortalClient
    {
        /// <summary>
        /// Method to call when TouchPortal is connected.
        /// </summary>
        Func<MessageInfo, Task> OnInfo { get; set; }

        /// <summary>
        /// Method to call when an item is selected from dropdown in Action Creation of a button.
        /// </summary>
        Func<MessageListChange, Task> OnListChanged { get; set; }

        /// <summary>
        /// ...
        /// </summary>
        Func<MessageBroadcast, Task> OnBroadcast { get; set; }

        /// <summary>
        /// ...
        /// </summary>
        Func<MessageSettings, Task> OnSettings { get; set; }

        /// <summary>
        /// Method to call when a user presses a button on their device.
        /// </summary>
        Func<MessageAction, Task> OnAction { get; set; }

        /// <summary>
        /// Method to call when we loose connection to TouchPortal.
        /// </summary>
        Action<Exception> OnClosed { get; set; }

        /// <summary>
        /// Messages that are unknown, and therefor we cannot deserialize to a known type.
        /// </summary>
        Func<JsonDocument, Task> OnUnhandled { get; set; }

        /// <summary>
        /// Connects, pairs, and listens to the TouchPortal application.
        /// </summary>
        /// <returns>connection success status</returns>
        Task<bool> Connect();
        
        bool Listen();

        /// <summary>
        /// Closes the connection to TouchPortal and shutdowns the plugin in a safe manner.
        /// </summary>
        /// <param name="exception">optional exception of why we want to close the connection.</param>
        void Close(Exception exception = default);

        /// <summary>
        /// Creates a dynamic state in TouchPortal Memory.
        /// This state will disappear when restarting TouchPortal.
        /// You will need to persist them yourself and reload them on plugin load.
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="displayName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        Task<bool> CreateState(string stateId, string displayName, string defaultValue = "");

        /// <summary>
        /// Updates a setting in TouchPortal.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> SettingUpdate(string name, string value);

        /// <summary>
        /// Removes the dynamic state from TouchPortal.
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        Task<bool> RemoveState(string stateId);

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
        Task<bool> StateUpdate(string stateId, string value);

        /// <summary>
        /// Updates the drop down choices in the TouchPortal UI.
        /// InstanceId can be used to dynamically update a dropdown based on the value chosen from another dropdown.
        /// </summary>
        /// <param name="listId">Id of UI dropdown.</param>
        /// <param name="values">Values as string array that you can choose from.</param>
        /// <param name="instanceId">if set (fetched from listChange event), this will only update this particular list.</param>
        /// <returns></returns>
        Task<bool> ChoiceUpdate(string listId, string[] values, string instanceId = null);

        /// <summary>
        /// Updates the constraints of a data value.
        /// </summary>
        /// <param name="dataId">Id of action the number box.</param>
        /// <param name="minValue">Min value the field can be.</param>
        /// <param name="maxValue">Max value the field can be.</param>
        /// <param name="dataType">Type of the data field.</param>
        /// <param name="instanceId">if set (fetched from listChange event), this will only update this particular list.</param>
        /// <returns></returns>
        Task<bool> UpdateActionData(string dataId, double minValue, double maxValue, DataType dataType, string instanceId = null);
    }
}