using TimestampLogger.Models;
using TimestampLogger.Services;

namespace TimestampLogger.Processors
{
    public class MessageProcessorB : IMessageProcessor<BodypixMessageType>
    {
        private readonly MessageMatchingService _matchingService;

        public MessageProcessorB(MessageMatchingService matchingService)
        {
            _matchingService = matchingService;
        }

        public void ProcessMessage(BodypixMessageType message)
        {
            _matchingService.AddOrUpdateMessageB(message);
        }
    }
}