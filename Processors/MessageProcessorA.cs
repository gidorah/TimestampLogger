using TimestampLogger.Models;
using TimestampLogger.Services;

namespace TimestampLogger.Processors
{
    public class MessageProcessorA : IMessageProcessor<OrionMessageType>
    {
        private readonly MessageMatchingService _matchingService;

        public MessageProcessorA(MessageMatchingService matchingService)
        {
            _matchingService = matchingService;
        }

        public void ProcessMessage(OrionMessageType message)
        {
            _matchingService.AddOrUpdateMessageA(message);
        }
    }
}