using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Models;

namespace TouchPortalSDK.Utils
{
    public class CommandStore : ICommandStore
    {
        private readonly TouchPortalOptions _options;
        private readonly ILogger<CommandStore> _logger;

        public CommandStore(TouchPortalOptions options,
                            ILogger<CommandStore> logger = null)
        {
            _options = options;
            _logger = logger;
        }

        ///<inheritdoc cref="ICommandStore"/>
        public void StoreCommands(string pluginId, IReadOnlyCollection<string> messages)
        {
            if(!_options.LogCommands)
                return;

            _logger?.LogInformation($"Logging '{messages.Count}' commands sent during this session.");

            try
            {
                var lines = messages
                    .Select(Encoding.UTF8.GetBytes)
                    .Select(Convert.ToBase64String);

                var filename = SanitizeFileName(pluginId);
                File.WriteAllLines(filename, lines);
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, "Could not write to command log file.");
            }
        }

        ///<inheritdoc cref="ICommandStore"/>
        public IReadOnlyCollection<string> LoadCommands(string pluginId)
        {
            if (!_options.RestoreCommands)
                return Array.Empty<string>();
            
            try
            {
                var filename = SanitizeFileName(pluginId);
                var commands =  File.ReadAllLines(filename)
                    .Select(Convert.FromBase64String)
                    .Select(Encoding.UTF8.GetString)
                    .ToArray();

                _logger?.LogInformation($"[RestoreState] Restoring state from '{commands.Length}' commands.");
                return commands;
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, "Could not read from command log file.");
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Removes illegal characters from the filename, dependent on OS.
        /// </summary>
        private static string SanitizeFileName(string pluginId)
        {
            const string extension = ".clog";
            var stringBuilder = new StringBuilder(pluginId.Length + extension.Length);

            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var ch in pluginId)
                stringBuilder.Append(invalidChars.Any(invalidChar => invalidChar == ch) ? '_' : ch);

            stringBuilder.Append(extension);

            return stringBuilder.ToString();
        }
    }
}
