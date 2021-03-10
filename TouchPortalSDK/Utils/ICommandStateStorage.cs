using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Models;
using TouchPortalSDK.Utils.TouchPortalSDK.Models;

namespace TouchPortalSDK.Utils
{
    namespace TouchPortalSDK.Models
    {
        //TODO: CustomStates? //Tracked by plugin. Must be refreshed on boot. Plugin needs to persist.
        //TODO: Settings? Tracket by TouchPortal in registry under: HKEY_CURRENT_USER\SOFTWARE\JavaSoft\Prefs\app\core\utilities\ <plugin name folder> \
        //TODO: DataConstraints? //Needs to be tracked as entry.tp + updates
        public class CustomPluginUpdates
        {
            //Stored on created actions, but in memory for new ones:
            //Ex. %AppData%pages\(main).tml
            public Dictionary<string, string> ActionData { get; } = new Dictionary<string, string>();
            //In Mem:
            public Dictionary<string, string> States { get; } = new Dictionary<string, string>();

            //Stored in registry:
            //HKEY_CURRENT_USER\SOFTWARE\JavaSoft\Prefs\app\core\utilities\ <plugin name folder> \
            public Dictionary<string, string> Settings { get; } = new Dictionary<string, string>();

            //In Mem:
            public Dictionary<string, string> Choices { get; } = new Dictionary<string, string>();
        }
    }

    public interface ICommandStateStorage
    {
        void Store(CustomPluginUpdates customPluginUpdates);
        CustomPluginUpdates Load();
    }

    public class CommandStateStorage
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<CommandStateStorage> _logger;
        private readonly TouchPortalOptions _options;

        public CommandStateStorage(TouchPortalOptions options,
                                   ILogger<CommandStateStorage> logger = null)
        {
            _logger = logger;
            _options = options;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true
            };
        }

        public void Store(CustomPluginUpdates customPluginUpdates)
        {
            //if (!_options.StoreCustomUpdates)
            //    return;

            try
            {
                var json = JsonSerializer.Serialize(customPluginUpdates, _serializerOptions);
                File.WriteAllText("customstates.json", json);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Could not store state.");
            }
        }

        public CustomPluginUpdates Load()
        {
            //if (!_options.LoadCustomUpdates)
            //    return new CustomPluginUpdates();

            try
            {
                var json = File.ReadAllText("customstates.json");
                return JsonSerializer.Deserialize<CustomPluginUpdates>(json, _serializerOptions) ?? new CustomPluginUpdates();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Could not load state.");
                return new CustomPluginUpdates();
            }
        }
    }
}
