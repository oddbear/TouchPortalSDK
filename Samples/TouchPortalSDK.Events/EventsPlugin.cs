using Microsoft.Extensions.Logging;
using SampleUtils;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Commands;

namespace TouchPortalSDK.Events
{
    public class EventsPlugin : SamplePluginBase
    {
        protected override ILogger _logger { get; }
        protected override ITouchPortalClient _client { get; }

        public EventsPlugin(
            ITouchPortalClientFactory clientFactory,
            ILogger<EventsPlugin> logger)
        {
            _logger = logger;
            _client = clientFactory.Create(this);
        }

        public void Run()
        {
            //Connect to Touch Portal:
            _client.Connect();

            //Examples, set a random state value and the event should be triggered:
            var states = new[] { "Apple", "Pears", "Grapes", "Bananas" };
            var randomIndex = Random.Shared.Next(0, states.Length);
            var state = states[randomIndex];

            //You can also send custom messages to Touch Portal, if this SDK gets outdated,
            // then this is an option (and receiving with the OnUnhandledEvent).
            var command = StateUpdateCommand.CreateAndValidate("fruit", state);
            _client.SendCommand(command);

            //Or as message:
            //_client.SendMessage(JsonSerializer.Serialize(command));
        }
    }
}
