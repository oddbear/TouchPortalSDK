using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Models.Enums;
using TouchPortalSDK.Models.Messages;

namespace TouchPortalSDK.Sample
{
    class Program
    {
        private static ILogger _logger;
        private static ITouchPortalClient _client;

        static void Main(string[] args)
        {
            //Standard method for build a ServiceProvider in .Net,
            // you can use any other IoC container, or no at all if you want:
            var serviceProvider = Startup.CreateServiceProvider();

            //Ask provider for services:
            _logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            _client = serviceProvider.GetRequiredService<ITouchPortalClient>();

            //Setting callback actions:
            _client.OnInfo = OnInfo;
            _client.OnAction = OnAction;
            _client.OnListChanged = OnListChanged;
            _client.OnUnhandled = document => _logger.LogWarning($"Unhandled message: {document}");
            _client.OnBroadcast = broadcast => _logger.LogInformation($"[Broadcast] Event: '{broadcast.Event}', PageName: '{broadcast.PageName}'");
            _client.OnSettings = OnSettings;
            _client.OnClosed = exception =>
            {
                _logger.LogInformation(exception, "TouchPortal Disconnected.");
                //Optional force exits this program.
                Environment.Exit(0);
            };

            //Connect to TouchPortal:
            _client.Connect();
            
            //Update choices (dropdown in UI when creating an action):
            _client.ChoiceUpdate("category1.action1.data2", new[] { "choice 1 (updated)", "choice 2 (updated)", "choice 3 (updated)" });

            //Removes a dynamic state (no change if state does not exist):
            _client.RemoveState("dynamicState1");

            //Adds a state we can work with:
            _client.CreateState("dynamicState1", "Test dynamic state 1", "Test 123");

            //Updates the created dynamic state, if you do not create it:
            _client.StateUpdate("dynamicState1", "d1");

            //You can display this value, but it will not appear in any list:
            _client.StateUpdate("dynamicState2", "d2");

            //Updates the static state (entry.tp):
            _client.StateUpdate("category1.staticstate1", "s1");

            //Custom states (Global Objects/left panel in TouchPortal UI), user adds this (states.tp in %AppData%/TouchPortal).
            //The user should add this manually in the UI:
            _client.StateUpdate("global.customState1", "c2");

            //Updates settings in TouchPortal settings:
            _client.SettingUpdate("Test3", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));

            //Updates the min and max value of the number field.
            _client.UpdateActionData("category1.action1.data4", 10, 15, DataType.Number);
        }

        /// <summary>
        /// Information received when plugin is connected to TouchPortal.
        /// </summary>
        /// <param name="message"></param>
        private static void OnInfo(MessageInfo message)
        {
            var settings = string.Join(", ", message.Settings.Select(dataItem => $"\"{dataItem.Name}\":\"{dataItem.Value}\""));
            _logger.LogInformation($"[Info] VersionCode: '{message.TpVersionCode}', VersionString: '{message.TpVersionString}', SDK: '{message.SdkVersion}', PluginVersion: '{message.PluginVersion}', Status: '{message.Status}', Settings: '{settings}'");
        }

        /// <summary>
        /// User selected an item in a dropdown menu in the TouchPortal UI.
        /// </summary>
        /// <param name="message"></param>
        private static void OnListChanged(MessageListChange message)
        {
            _logger.LogInformation($"[OnListChanged] {message.ListId}/{message.ActionId}/{message.InstanceId} '{message.Value}'");

            switch (message.ListId)
            {
                //Dynamically updates the dropdown of data3 based on value chosen from data2 dropdown:
                case "category1.action1.data2" when message.InstanceId is not null:
                    var prefix = message.Value;
                    _client.ChoiceUpdate("category1.action1.data3", new[] { $"{prefix} second 1", $"{prefix} second 2", $"{prefix} second 3" }, message.InstanceId);
                    break;
            }
        }

        /// <summary>
        /// User clicked an action.
        /// </summary>
        /// <param name="message"></param>
        private static void OnAction(MessageAction message)
        {
            switch (message.ActionId)
            {
                case "category1.action1":
                    //Get data with indexer:
                    var data1 = message["category1.action1.data1"] ?? "<null>";
                    var data2 = message["category1.action1.data2"] ?? "<null>";
                    var data3 = message["category1.action1.data3"] ?? "<null>";
                    var data4 = message["category1.action1.data4"] ?? "<null>";
                    //Get date with method:
                    var data5 = message.GetValue("category1.action1.data5") ?? "<null>";
                    var data6 = message.GetValue("category1.action1.data6") ?? "<null>";
                    var data7 = message.GetValue("category1.action1.data7") ?? "<null>";
                    var data8 = message.GetValue("category1.action1.data8") ?? "<null>";
                    _logger.LogInformation($"[OnAction] PressState: {message.GetPressState()}, ActionId: {message.ActionId}, Data: data1:'{data1}', data2:'{data2}', data3:'{data3}', data4:'{data4}', data5:'{data5}', data6:'{data6}', data7:'{data7}', data8:'{data8}'");
                    break;

                default:
                    var data = string.Join(", ", message.Data.Select(dataItem => $"\"{dataItem.Id}\":\"{dataItem.Value}\""));
                    _logger.LogInformation($"[OnAction] PressState: {message.GetPressState()}, ActionId: {message.ActionId}, Data: '{data}'");
                    break;
            }
        }

        private static void OnSettings(MessageSettings message)
        {
            var settings = string.Join(", ", message.Values.Select(dataItem => $"\"{dataItem.Name}\":\"{dataItem.Value}\""));
            _logger.LogInformation($"[OnSettings] '{settings}'");
        }
    }
}
