using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using TimestampLogger.Models;


namespace TimestampLogger.Services
{
    public class MessageMatchingService
    {
        private readonly ConcurrentDictionary<String, (OrionMessageType? messageA, BodypixMessageType? messageB)> _messages = new ConcurrentDictionary<String, (OrionMessageType? messageA, BodypixMessageType? messageB)>();
        private readonly ILogger<MessageMatchingService> _logger;

        public MessageMatchingService(ILogger<MessageMatchingService> logger)
        {
            _logger = logger;
        }

        public void AddOrUpdateMessageA(OrionMessageType message)
        {
            _messages.AddOrUpdate(message.weaponTrigger,
                (message, null), // Add
                (_, existing) => (message, existing.messageB)); // Update
            TryLogMatchedMessage(message.weaponTrigger);
        }

        public void AddOrUpdateMessageB(BodypixMessageType message)
        {
            string v = message.TriggerTimestamp.ToString();

            _messages.AddOrUpdate(v,
                (null, message), // Add
                (_, existing) => (existing.messageA, message)); // Update
            TryLogMatchedMessage(v);
        }

        private void TryLogMatchedMessage(String weaponTrigger)
        {
            if (_messages.TryGetValue(weaponTrigger, out var pair) && pair.messageA != null && pair.messageB != null)
            {
                _logger.LogInformation($"Matched messages for {weaponTrigger}: A: {pair.messageA.weaponTrigger}, B: {pair.messageB.TriggerTimestamp}");
                _messages.TryRemove(weaponTrigger, out _); // Remove after logging
            }
        }
    }
}