using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

public class RedisListener
{
    private readonly ILogger<RedisListener> _logger;
    private readonly IConfiguration _configuration;
    private ConnectionMultiplexer _redisConnection;
    private ISubscriber _subscriber;

    public RedisListener(ILogger<RedisListener> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task ListenAsync(string channelName, CancellationToken stoppingToken)
    {
        try
        {
            _redisConnection = await ConnectionMultiplexer.ConnectAsync(_configuration["Redis:ConnectionString"]);
            _subscriber = _redisConnection.GetSubscriber();

            await _subscriber.SubscribeAsync(channelName, (channel, message) =>
            {
                _logger.LogInformation($"Received message: {message}");
            });

            _logger.LogTrace($"Subscribed to Redis channel '{channelName}'. Listening for incoming messages...");

            // Keep the listener alive until a stopping signal is received
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken); // Wait for 1 second
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while listening to Redis messages.");
        }
        finally
        {
            await UnsubscribeAsync(channelName);
        }
    }

    private async Task UnsubscribeAsync(string channelName)
    {
        if (_subscriber != null)
        {
            await _subscriber.UnsubscribeAsync(channelName);
            _logger.LogInformation($"Unsubscribed from Redis channel '{channelName}'");
        }

        _redisConnection?.Close();
    }
}
