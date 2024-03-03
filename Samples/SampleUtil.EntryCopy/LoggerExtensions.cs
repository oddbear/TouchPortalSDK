using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace SampleUtils
{
    public static class LoggerExtensions
    {
        public static void LogObjectAsJson(this ILogger? logger, object? message, [CallerMemberName] string memberName = "")
        {
            if (logger is null || message is null)
                return;

            var json = JsonSerializer.Serialize(message);
            logger.LogInformation($"{memberName}: {json}");
        }
    }
}
