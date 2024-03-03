using System.Linq;
using Microsoft.Extensions.Logging;
using SampleUtils;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;

namespace TouchPortalSDK.Actions
{
    public class ActionsPlugin : SamplePluginBase
    {
        protected override ILogger Logger { get; }
        protected override ITouchPortalClient Client { get; }

        public ActionsPlugin(
            ITouchPortalClientFactory clientFactory,
            ILogger<ActionsPlugin> logger)
        {
            Logger = logger;
            Client = clientFactory.Create(this);
        }

        public override void OnActionEvent(ActionEvent message)
        {
            // Example: if a button is triggered, it can either be with, or without data:
            // There are 3 types of actions:
            // action:
            // down:
            // up:
            Logger.LogObjectAsJson(message);
        }

        public override void OnListChangedEvent(ListChangeEvent message)
        {
            Logger.LogObjectAsJson(message);

            // Example: One list triggers an update of another list
            // , when user changes the value of the first list in Touch Portal.
            if (message.ListId == "action.with.listUpdates.list1")
            {
                var values = Enumerable.Repeat(message.Value, 4).ToArray();
                Client.ChoiceUpdate("action.with.listUpdates.list2", values, message.InstanceId);
            }
        }

        public void Run()
        {
            // Connect to Touch Portal:
            Client.Connect();

            // Example: Updates all instances of the list with new values.
            var values = new[] { "a", "b", "c" };
            Client.ChoiceUpdate("action.with.listUpdates.list2", values);
        }
    }
}
