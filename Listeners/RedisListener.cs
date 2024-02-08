using StackExchange.Redis;
using System.Text.Json;
using TimestampLogger.Processors;
using Microsoft.Extensions.Logging;

namespace TimestampLogger.Listeners
{
    public class RedisListener<T>
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IMessageProcessor<T> _processor;
        private readonly ILogger<RedisListener<T>> _logger;
        private readonly string _channelName;

        public RedisListener(IConnectionMultiplexer redis, IMessageProcessor<T> processor, ILogger<RedisListener<T>> logger, string channelName)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _channelName = channelName ?? throw new ArgumentNullException(nameof(channelName));
        }

        public void Start()
        {
            var subscriber = _redis.GetSubscriber();
            subscriber.Subscribe(_channelName, (channel, message) =>
            {
                try
                {
                    var messageObj = JsonSerializer.Deserialize<T>(message);
                    if (messageObj != null)
                    {
                        _processor.ProcessMessage(messageObj);
                    }
                    else
                    {
                        _logger.LogWarning("Received message is null after deserialization.");
                    }
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Error deserializing message from channel {ChannelName}", _channelName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception processing message from channel {ChannelName}", _channelName);
                }
            });

            _logger.LogInformation("Subscribed to Redis channel '{ChannelName}'", _channelName);
        }
    }
}
