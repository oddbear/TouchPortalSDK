using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Configuration;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Messages.Items;
using TouchPortalSDK.Models.Enums;
using TouchPortalSDK.Utils;

namespace TouchPortalSDK.Sample
{
    public class SamplePlugin
    {
        public string PluginId => "TouchPortalSDK.Sample";
        
        private readonly ILogger<SamplePlugin> _logger;
        private readonly ITouchPortalClient _client;

        private IReadOnlyCollection<Setting> _settings;

        public SamplePlugin(ITouchPortalClientFactory clientFactory,
                            ILogger<SamplePlugin> logger)
        {
            _logger = logger;
            _client = clientFactory.Create(new ParallelEventHandler(PluginId, 5)
            {
                OnClosedEventAsync = OnClosedEvent,
                OnActionEventAsync = OnActionEvent,
                OnBroadcastEventAsync = OnBroadcastEvent,
                OnInfoEventAsync = OnInfoEvent,
                OnListChangedEventAsync = OnListChangedEvent,
                OnSettingsEventAsync = OnSettingsEvent,
                OnUnhandledEventAsync = OnUnhandledEvent
            });
        }

        public void Run()
        {
            //Connect to TouchPortal:
            _client.Connect();

            //Start sending messages:
            SendMessages();
        }

        public Task OnClosedEvent(string message)
        {
            _logger?.LogInformation("TouchPortal Disconnected.");
            
            //Optional force exits this plugin.
            Environment.Exit(0);

            return Task.CompletedTask;
        }

        private void SendMessages()
        {
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
            _client.UpdateActionData("category1.action1.data4", 10, 15, ActionDataType.Number);
            
            //_client.Close()
        }

        /// <summary>
        /// Information received when plugin is connected to TouchPortal.
        /// </summary>
        /// <param name="message"></param>
        public Task OnInfoEvent(InfoEvent message)
        {
            _logger?.LogInformation($"[Info] VersionCode: '{message.TpVersionCode}', VersionString: '{message.TpVersionString}', SDK: '{message.SdkVersion}', PluginVersion: '{message.PluginVersion}', Status: '{message.Status}'");

            _settings = message.Settings;
            _logger?.LogInformation($"[Info] Settings: {JsonSerializer.Serialize(_settings)}");

            return Task.CompletedTask;
        }

        /// <summary>
        /// User selected an item in a dropdown menu in the TouchPortal UI.
        /// </summary>
        /// <param name="message"></param>
        public Task OnListChangedEvent(ListChangeEvent message)
        {
            _logger?.LogInformation($"[OnListChanged] {message.ListId}/{message.ActionId}/{message.InstanceId} '{message.Value}'");

            switch (message.ListId)
            {
                //Dynamically updates the dropdown of data3 based on value chosen from data2 dropdown:
                case "category1.action1.data2" when message.InstanceId is not null:
                    var prefix = message.Value;
                    _client.ChoiceUpdate("category1.action1.data3", new[] { $"{prefix} second 1", $"{prefix} second 2", $"{prefix} second 3" }, message.InstanceId);
                    break;
            }

            return Task.CompletedTask;
        }

        public Task OnBroadcastEvent(BroadcastEvent message)
        {
            _logger?.LogInformation($"[Broadcast] Event: '{message.Event}', PageName: '{message.PageName}'");

            return Task.CompletedTask;
        }

        public Task OnSettingsEvent(SettingsEvent message)
        {
            _settings = message.Values;
            _logger?.LogInformation($"[OnSettings] Settings: {JsonSerializer.Serialize(_settings)}");

            return Task.CompletedTask;
        }

        /// <summary>
        /// User clicked an action.
        /// </summary>
        /// <param name="message"></param>
        public async Task OnActionEvent(ActionEvent message)
        {
            await Task.Delay(5000);

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
                    _logger?.LogInformation($"[OnAction] PressState: {message.GetPressState()}, ActionId: {message.ActionId}, Data: data1:'{data1}', data2:'{data2}', data3:'{data3}', data4:'{data4}', data5:'{data5}', data6:'{data6}', data7:'{data7}', data8:'{data8}'");
                    break;

                default:
                    var data = string.Join(", ", message.Data.Select(dataItem => $"\"{dataItem.Id}\":\"{dataItem.Value}\""));
                    _logger?.LogInformation($"[OnAction] PressState: {message.GetPressState()}, ActionId: {message.ActionId}, Data: '{data}'");
                    break;
            }
        }

        public Task OnUnhandledEvent(string jsonMessage)
        {
            var jsonDocument = JsonSerializer.Deserialize<JsonDocument>(jsonMessage);
            _logger?.LogWarning($"Unhandled message: {jsonDocument}");

            return Task.CompletedTask;
        }
    }
}
