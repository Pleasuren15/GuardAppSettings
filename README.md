# GuardAppSettings

A CLI tool that generates strongly typed C# classes from `appsettings.json` files, allowing you to access configuration values with compile-time safety and IntelliSense support.

## Installation

To install as a global .NET tool:

```bash
dotnet pack
dotnet tool install -g --add-source ./src/GuardAppSettings/bin/Debug GuardAppSettings
```

## Usage

```bash
dotnet gas create --project <path-to-appsettings-directory> --target <output-directory>
```

### Parameters

- `--project`: Path to the directory containing your `appsettings.json` file
- `--target`: Path to the directory where the generated `AppSettings.cs` class will be created

### Example

```bash
dotnet gas create --project ./MyProject --target ./MyProject/Configuration
```

This command will:
1. Read `appsettings.json` from the `./MyProject` directory
2. Generate a strongly typed `AppSettings.cs` class
3. Save it to `./MyProject/Configuration/AppSettings.cs`

## Generated Code Features

The generated `AppSettings` class includes:

- **Constructor injection**: Accepts `IConfiguration` through dependency injection
- **Strongly typed properties**: Each configuration value becomes a typed property
- **Nested classes**: Complex JSON objects become nested configuration classes
- **Type safety**: Automatic type conversion with safe defaults
- **Array support**: JSON arrays become typed arrays in C#
- **Property name sanitization**: Handles properties with dots, spaces, and special characters
- **Proper indentation**: Clean, readable generated code
- **Missing imports**: Includes all necessary using statements

## Example

Given this `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyApp;Trusted_Connection=true;",
    "Redis": "localhost:6379"
  },
  "ApiSettings": {
    "BaseUrl": "https://api.example.com",
    "Timeout": 30,
    "RetryCount": 3,
    "EnableCaching": true
  },
  "FeatureFlags": [
    "NewFeature",
    "BetaFeature"
  ]
}
```

The tool generates:

```csharp
using Microsoft.Extensions.Configuration;

namespace AppSettings;

public class AppSettings
{
    private readonly IConfiguration _configuration;

    public AppSettings(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ConnectionStringsSettings ConnectionStrings => new ConnectionStringsSettings(_configuration);
    public ApiSettingsSettings ApiSettings => new ApiSettingsSettings(_configuration);
    public string[] FeatureFlags => _configuration.GetSection("FeatureFlags").Get<string[]>() ?? Array.Empty<string>();

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
}
```

## Usage in Your Application

1. Register the generated class in your DI container:

```csharp
services.AddSingleton<AppSettings.AppSettings>();
```

2. Inject and use it in your services:

```csharp
public class MyService
{
    private readonly AppSettings.AppSettings _appSettings;

    public MyService(AppSettings.AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public void DoSomething()
    {
        var connectionString = _appSettings.ConnectionStrings.DefaultConnection;
        var apiUrl = _appSettings.ApiSettings.BaseUrl;
        var isFeatureEnabled = _appSettings.FeatureFlags.Contains("NewFeature");
    }
}
```

## Supported Types

- **Strings**: Mapped to `string` properties with empty string defaults
- **Numbers**: Mapped to `int` or `double` based on the JSON value
- **Booleans**: Mapped to `bool` properties
- **Objects**: Mapped to nested classes with the same pattern
- **Arrays**: Mapped to typed arrays (currently supports string arrays)

## Requirements

- .NET 9.0 or later
- Microsoft.Extensions.Configuration package in your target project