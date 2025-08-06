using Microsoft.Extensions.Configuration;

namespace GuardAppSettings.Tests;

public class AppSettings
{
    private readonly IConfiguration _configuration;

    public AppSettings(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ConnectionStringsSettings ConnectionStrings => new ConnectionStringsSettings(_configuration);

    public class ConnectionStringsSettings
    {
        private readonly IConfiguration _configuration;

        public ConnectionStringsSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string DefaultConnection => _configuration["ConnectionStrings:DefaultConnection"] ?? string.Empty;
        public string Redis => _configuration["ConnectionStrings:Redis"] ?? string.Empty;
    }

    public LoggingSettings Logging => new LoggingSettings(_configuration);

    public class LoggingSettings
    {
        private readonly IConfiguration _configuration;

        public LoggingSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public LogLevelSettings LogLevel => new LogLevelSettings(_configuration);

        public class LogLevelSettings
        {
            private readonly IConfiguration _configuration;

            public LogLevelSettings(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public string Default => _configuration["Logging:LogLevel:Default"] ?? string.Empty;
            /// <summary>Configuration key: "Logging:LogLevel:Microsoft.AspNetCore"</summary>
            public string MicrosoftAspNetCore => _configuration["Logging:LogLevel:Microsoft.AspNetCore"] ?? string.Empty;

            /// <summary>Access configuration values by exact key name (useful for keys with special characters)</summary>
            public string this[string key] => _configuration["Logging:LogLevel:" + key] ?? string.Empty;
        }

    }

    public ApiSettingsSettings ApiSettings => new ApiSettingsSettings(_configuration);

    public class ApiSettingsSettings
    {
        private readonly IConfiguration _configuration;

        public ApiSettingsSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string BaseUrl => _configuration["ApiSettings:BaseUrl"] ?? string.Empty;
        public int Timeout => int.TryParse(_configuration["ApiSettings:Timeout"], out var value) ? value : 0;
        public int RetryCount => int.TryParse(_configuration["ApiSettings:RetryCount"], out var value) ? value : 0;
        public bool EnableCaching => bool.TryParse(_configuration["ApiSettings:EnableCaching"], out var value) && value;
    }

    public string[] FeatureFlags => _configuration.GetSection("FeatureFlags").GetChildren().Select(c => c.Value ?? string.Empty).ToArray();
}
