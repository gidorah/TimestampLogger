using StackExchange.Redis;

namespace TimestampLogger.Services
{
    public class RedisService : IRedisService
    {
        private readonly string _redisConnectionString;
        private IConnectionMultiplexer _connectionMultiplexer;

        public RedisService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        }

        public async Task<IConnectionMultiplexer> ConnectAsync()
        {
            if (_connectionMultiplexer == null || !_connectionMultiplexer.IsConnected)
            {
                _connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(_redisConnectionString);
            }
            return _connectionMultiplexer;
        }

        public void Subscribe(string channelName, Action<RedisChannel, RedisValue> handler)
        {
            if (string.IsNullOrWhiteSpace(channelName)) throw new ArgumentException("Channel name cannot be null or whitespace.", nameof(channelName));

            var subscriber = _connectionMultiplexer.GetSubscriber();
            subscriber.Subscribe(channelName, (channel, value) => handler(channel, value));
        }

        public async Task PublishAsync(string channelName, string message)
        {
            if (string.IsNullOrWhiteSpace(channelName)) throw new ArgumentException("Channel name cannot be null or whitespace.", nameof(channelName));
            if (message == null) throw new ArgumentNullException(nameof(message));

            var subscriber = _connectionMultiplexer.GetSubscriber();
            await subscriber.PublishAsync(channelName, message);
        }
    }
}
