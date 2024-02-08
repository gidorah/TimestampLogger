using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using StackExchange.Redis;
using TimestampLogger.Listeners;
using TimestampLogger.Models;
using TimestampLogger.Processors;
using TimestampLogger.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


var builder = Host.CreateDefaultBuilder(args);

// Configure Serilog
builder.UseSerilog((hostContext, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(hostContext.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day);
});

builder.ConfigureServices((hostContext, services) =>
{
    // Configuration for services as in the previous example
    var redisConfiguration = hostContext.Configuration.GetConnectionString("Redis");
    services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration));
    services.AddSingleton<IRedisService, RedisService>(provider =>
        new RedisService(provider.GetRequiredService<IConnectionMultiplexer>()));

    services.AddSingleton<MessageMatchingService>();
    services.AddSingleton<IMessageProcessor<OrionMessageType>, MessageProcessorA>();
    services.AddSingleton<IMessageProcessor<BodypixMessageType>, MessageProcessorB>();

    services.AddSingleton<RedisListener<OrionMessageType>>(provider =>
        new RedisListener<OrionMessageType>(
            provider.GetRequiredService<IConnectionMultiplexer>(),
            provider.GetRequiredService<IMessageProcessor<OrionMessageType>>(),
            provider.GetRequiredService<ILogger<RedisListener<OrionMessageType>>>(),
            "led_vibration_timestamps"));
    services.AddSingleton<RedisListener<BodypixMessageType>>(provider =>
        new RedisListener<BodypixMessageType>(
            provider.GetRequiredService<IConnectionMultiplexer>(),
            provider.GetRequiredService<IMessageProcessor<BodypixMessageType>>(),
            provider.GetRequiredService<ILogger<RedisListener<BodypixMessageType>>>(),
            "bp"));


    services.AddHostedService<RedisListenerHostedService>();
});

var app = builder.Build();
app.Run();

public class RedisListenerHostedService : IHostedService
{
    private readonly RedisListener<OrionMessageType> _listenerA;
    private readonly RedisListener<BodypixMessageType> _listenerB;

    public RedisListenerHostedService(
        RedisListener<OrionMessageType> listenerA,
        RedisListener<BodypixMessageType> listenerB)
    {
        _listenerA = listenerA;
        _listenerB = listenerB;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _listenerA.Start();
        _listenerB.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Implement any cleanup or shutdown logic here if necessary
        return Task.CompletedTask;
    }
}

// bp {"deviceId":3,"deviceType":1,"timestamp":1707382177138,"arucoId":-1,"targetPersonIndex":0,"numOfPerson":1,"label":0}

// 