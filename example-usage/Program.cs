using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExampleApp;

class Program
{
    static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Register the generated AppSettings class
                services.AddSingleton<AppSettings.AppSettings>();
                services.AddScoped<MyService>();
            })
            .Build();

        var myService = host.Services.GetRequiredService<MyService>();
        myService.DoWork();
    }
}

public class MyService
{
    private readonly AppSettings.AppSettings _appSettings;

    public MyService(AppSettings.AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public void DoWork()
    {
        // Access strongly typed configuration values
        Console.WriteLine($"Database Connection: {_appSettings.ConnectionStrings.DefaultConnection}");
        Console.WriteLine($"Redis Connection: {_appSettings.ConnectionStrings.Redis}");
        Console.WriteLine($"API Base URL: {_appSettings.ApiSettings.BaseUrl}");
        Console.WriteLine($"API Timeout: {_appSettings.ApiSettings.Timeout} seconds");
        Console.WriteLine($"Caching Enabled: {_appSettings.ApiSettings.EnableCaching}");
        Console.WriteLine($"Log Level: {_appSettings.Logging.LogLevel.Default}");
        
        // Access array values
        var features = _appSettings.FeatureFlags;
        Console.WriteLine($"Feature Flags: {string.Join(", ", features)}");
        
        // Check if specific feature is enabled
        bool hasNewFeature = features.Contains("NewFeature");
        Console.WriteLine($"New Feature Enabled: {hasNewFeature}");
    }
}