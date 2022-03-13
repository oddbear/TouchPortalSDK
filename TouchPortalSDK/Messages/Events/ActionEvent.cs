using System.Collections.Generic;
using System.Linq;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Models;
using TouchPortalSDK.Messages.Models.Enums;

namespace TouchPortalSDK.Messages.Events
{
    public class ActionEvent : ITouchPortalEvent
    {
        /// <summary>
        /// <para>
        ///     Action type event.
        /// </para>
        /// <list type="bullet">
        /// <item>
        ///     <term>action</term>
        ///     <description>User presses an action button on the device.</description>
        /// </item>
        /// <item>
        ///     <term>down</term>
        ///     <description>Finger holds down the action on the device. This event happens only if the action enables the hasHoldFunctionality.</description>
        /// </item>
        /// <item>
        ///     <term>up</term>
        ///     <description>Finger released the action on the device. This event happens only if the action enables the hasHoldFunctionality.</description>
        /// </item>
        /// </list>
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The id of the plugin.
        /// </summary>
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
        public IReadOnlyCollection<Data> Data { get; set; }

        /// <summary>
        /// Indexer to get data values.
        /// </summary>
        /// <param name="dataId">the id of the datafield.</param>
        /// <returns>the value of the data field as string or null if not exists</returns>
        public string this[string dataId]
            => GetValue(dataId);

        /// <summary>
        /// Returns the value of the selected item in an action data field.
        /// This value can be null in some cases, and will be null if data field is miss written.
        /// </summary>
        /// <param name="dataId">the id of the datafield.</param>
        /// <returns>the value of the data field as string or null if not exists</returns>
        public string GetValue(string dataId)
            => Data?.SingleOrDefault(data => data.Id == dataId)?.Value;

        /// <summary>
        /// Returns the Action type.
        /// </summary>
        /// <returns><see cref="Press"/> enum</returns>
        public Press GetPressState()
            => Type switch
            {
                "up" => Press.Up,
                "down" => Press.Down,
                _ => Press.Tap
            };
    }
}
