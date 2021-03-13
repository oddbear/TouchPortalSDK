using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace TouchPortalSDK.Utils
{
    public class CommandStore : ICommandStore
    {
        private readonly ILogger<CommandStore> _logger;

        public CommandStore(ILogger<CommandStore> logger = null)
        {
            _logger = logger;
        }

        ///<inheritdoc cref="ICommandStore"/>
        public void StoreCommands(string stateFile, IReadOnlyCollection<string> messages)
        {
            _logger?.LogInformation($"Logging '{messages.Count}' commands sent during this session.");

            try
            {
                var lines = messages
                    .Select(Encoding.UTF8.GetBytes)
                    .Select(Convert.ToBase64String);

                var filename = SanitizeFileName(stateFile);
                File.WriteAllLines(filename, lines);
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, "Could not write to command log file.");
            }
        }

        ///<inheritdoc cref="ICommandStore"/>
        public IReadOnlyCollection<string> LoadCommands(string stateFile)
        {
            try
            {
                var filename = SanitizeFileName(stateFile);
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
        private static string SanitizeFileName(string fileName)
        {
            var stringBuilder = new StringBuilder(fileName.Length);

            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var ch in fileName)
                stringBuilder.Append(invalidChars.Any(invalidChar => invalidChar == ch) ? '_' : ch);
            
            return stringBuilder.ToString();
        }
    }
}
