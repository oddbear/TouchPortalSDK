using Microsoft.Extensions.Logging;
using SampleUtils;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Commands;

namespace TouchPortalSDK.Events
{
    public class EventsPlugin : SamplePluginBase
    {
        protected override ILogger Logger { get; }
        protected override ITouchPortalClient Client { get; }

        public EventsPlugin(
            ITouchPortalClientFactory clientFactory,
            ILogger<EventsPlugin> logger)
        {
            Logger = logger;
            Client = clientFactory.Create(this);
        }

        public void Run()
        {
            // Connect to Touch Portal:
            Client.Connect();

            // Examples, set a random state value and the event should be triggered:
            var states = new[] { "Apple", "Pears", "Grapes", "Bananas" };
            var randomIndex = Random.Shared.Next(0, states.Length);
            var state = states[randomIndex];

            // You can also send custom messages to Touch Portal, if this SDK gets outdated,
            // then this is an option (and receiving with the OnUnhandledEvent).
            var command = StateUpdateCommand.CreateAndValidate("fruit", state);
            Client.SendCommand(command);

            // Or as message:
            // _client.SendMessage(JsonSerializer.Serialize(command));

            Task.Run(() =>
            {
                while (true)
                {
                    foreach (var s in states)
                    {
                        // TODO: How does these events work?
                        //Thread.Sleep(2500);
                        //Client.StateListUpdate("fruit", ["Value1", "Value2", "Value3"]);
                        //Client.SendMessage("""{"type":"stateListUpdate", "id":"tp_sid_fruit", "value":["Value1", "Value2", "Value3"]}""");
                        Thread.Sleep(2500);
                        //Client.TriggerEvent("event002", new Dictionary<string, string>
                        //{
                        //    ["fruit"] = s
                        //});
                        var success = Client.SendMessage($$"""{ "type" : "triggerEvent", "eventId" : "event002", "states": { "fruit": "{{s}}" } }""");
                    }
                }
            });
        }
    }
}
