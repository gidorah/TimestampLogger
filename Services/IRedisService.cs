using StackExchange.Redis;

namespace TimestampLogger.Services
{
    public interface IRedisService
    {
        /// <summary>
        /// Connects to the Redis server.
        /// </summary>
        /// <returns>The connection multiplexer to interact with Redis.</returns>
        Task<IConnectionMultiplexer> ConnectAsync();

        /// <summary>
        /// Subscribes to a specific Redis channel to listen for messages.
        /// </summary>
        /// <param name="channelName">The name of the channel to subscribe to.</param>
        /// <param name="handler">The action to perform when a message is received.</param>
        void Subscribe(string channelName, Action<RedisChannel, RedisValue> handler);

        /// <summary>
        /// Publishes a message to a specific Redis channel.
        /// </summary>
        /// <param name="channelName">The name of the channel to publish the message to.</param>
        /// <param name="message">The message to publish.</param>
        /// <returns>The task representing the asynchronous operation.</returns>
        Task PublishAsync(string channelName, string message);
    }
}
