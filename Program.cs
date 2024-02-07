using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/messages.log", rollingInterval: RollingInterval.Infinite)
    .CreateLogger();

        var host = CreateHostBuilder(args).Build();

        // Now the RedisListener can be retrieved and used with DI
        var listener = host.Services.GetRequiredService<RedisListener>();
        await listener.ListenAsync("messages", host.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping);

        await host.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                // Configure the app here if needed
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<RedisListener>();
            });
}
