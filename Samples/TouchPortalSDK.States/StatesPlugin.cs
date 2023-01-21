using Microsoft.Extensions.Logging;
using SampleUtils;
using TouchPortalSDK.Interfaces;

namespace TouchPortalSDK.States
{
    public class StatesPlugin : SamplePluginBase
    {
        protected override ILogger _logger { get; }
        protected override ITouchPortalClient _client { get; }

        public StatesPlugin(ITouchPortalClientFactory clientFactory,
                            ILogger<StatesPlugin> logger)
        {
            _logger = logger;
            _client = clientFactory.Create(this);
        }

        public void Run()
        {
            //Connect to Touch Portal:
            _client.Connect();

            //Examples, set a random state value:
            var states = new[] { "Apple", "Pears", "Grapes", "Bananas" };
            var randomIndex = Random.Shared.Next(0, states.Length);
            var state = states[randomIndex];
            _client.StateUpdate("tp_sid_fruit", state);

            //Dynamic states is not defined in the entry.tp file:
            _client.CreateState("dynamicState1", "dynamic state 1", DateTime.UtcNow.ToString("s"), "dynamic parentGroup");

            //You can update a dynamic state:
            //_client.StateUpdate("dynamicState1", DateTime.UtcNow.ToString("s");

            //You can delete a dynamic state:
            //_client.RemoveState("dynamicState1");
        }
    }
}
