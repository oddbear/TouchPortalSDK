using System;
using System.Collections.Generic;
using TouchPortalSDK.Interfaces;

namespace TouchPortalSDK.Messages.Commands
{
    public class TriggerEventCommand : ITouchPortalCommand
    {
        public string Type => "triggerEvent";

        public string EventId { get; set; }

        public Dictionary<string, string>? States { get; set; }

        public static TriggerEventCommand CreateAndValidate(string eventId, Dictionary<string, string>? states)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                throw new ArgumentNullException(nameof(eventId));

            var command = new TriggerEventCommand
            {
                EventId = eventId,
                States = states
            };

            return command;
        }
    }
}