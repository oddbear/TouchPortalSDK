namespace TouchPortalSDK.Models.Messages
{
    public class MessageBase
    {
        /// <summary>
        /// <para>
        ///     This is the types of messages that can be received from TouchPortal, and the plugin can react to.
        /// </para>
        /// <list type="bullet">
        /// <item>
        ///     <term>info</term>
        ///     <description>Message from TouchPortal upon connection.</description>
        /// </item>
        /// <item>
        ///     <term>closePlugin</term>
        ///     <description>TouchPortal closes/stops the plugin or shuts down.</description>
        /// </item>
        /// <item>
        ///     <term>listChange</term>
        ///     <description>When setting up an action in the TouchPortal UI. This event is fired if the user selects and item in the dropdown for a choice list.</description>
        /// </item>
        /// <item>
        ///     <term>broadcast</term>
        ///     <description>Waiting for 2.3</description>
        /// </item>
        /// <item>
        ///     <term>settings</term>
        ///     <description>Plugin settings changed in TouchPortal UI.</description>
        /// </item>
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
    }
}
