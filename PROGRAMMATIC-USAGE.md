# Running GuardAppSettings from C# Code

There are several ways to use GuardAppSettings programmatically in your C# applications:

## 1. Direct Library Usage (Recommended)

Add the library package reference:
```xml
<PackageReference Include="GuardAppSettings.Library" Version="1.0.0" />
```

Use the library directly in your code:

```csharp
using GuardAppSettings.Library;

// Generate from file
var code = GuardAppSettingsLibrary.GenerateFromFile("./appsettings.json");

// Generate and save to file
GuardAppSettingsLibrary.GenerateToFile("./appsettings.json", "./Generated/AppSettings.cs");

// Generate with custom namespace
var customCode = GuardAppSettingsLibrary.GenerateFromFile("./appsettings.json", "MyApp.Config");

// Generate from JSON string
var json = """{"Database": {"ConnectionString": "..."}}""";
var codeFromJson = GuardAppSettingsLibrary.GenerateFromJson(json);
```

## 2. Process Execution (CLI Tool)

Execute the CLI tool from your C# application:

```csharp
using System.Diagnostics;

var startInfo = new ProcessStartInfo
{
    FileName = "dotnet",
    Arguments = "gas create --project ./config --target ./output",
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false
};

using var process = Process.Start(startInfo);
await process.WaitForExitAsync();

if (process.ExitCode == 0)
{
    Console.WriteLine("Generation successful!");
}
```

## 3. File Watching and Auto-Regeneration

Automatically regenerate when appsettings.json changes:

```csharp
using GuardAppSettings.Library;

var watcher = new FileSystemWatcher("./")
{
    Filter = "appsettings.json",
    NotifyFilter = NotifyFilters.LastWrite
};

watcher.Changed += (sender, e) =>
{
    Thread.Sleep(100); // Wait for file write to complete
    GuardAppSettingsLibrary.GenerateToFile(e.FullPath, "./Generated/AppSettings.cs");
    Console.WriteLine("Regenerated AppSettings.cs");
};

watcher.EnableRaisingEvents = true;
```

## 4. Integration with Dependency Injection

Generate at application startup if needed:

```csharp
public static void ConfigureServices(IServiceCollection services)
{
    // Generate AppSettings.cs if it doesn't exist
    var outputPath = "./Generated/AppSettings.cs";
    if (!File.Exists(outputPath))
    {
        GuardAppSettingsLibrary.GenerateToFile("./appsettings.json", outputPath);
        Console.WriteLine("Generated AppSettings.cs - rebuild your project to use it");
    }

    // Register the generated class
    services.AddSingleton<AppSettings.AppSettings>();
}
```

## 5. MSBuild Integration

Add build-time generation to your .csproj file:

```xml
<!-- Import the MSBuild targets -->
<Import Project="GuardAppSettings.Build.props" />

<!-- Optional: Customize settings -->
<PropertyGroup>
  <GuardAppSettingsInput>$(ProjectDir)config\appsettings.json</GuardAppSettingsInput>
  <GuardAppSettingsOutput>$(ProjectDir)Models\AppSettings.cs</GuardAppSettingsOutput>
  <GuardAppSettingsNamespace>$(RootNamespace).Models</GuardAppSettingsNamespace>
</PropertyGroup>
```

This will automatically generate AppSettings.cs during build.

## 6. Advanced Scenarios

### Custom Code Generation
```csharp
// If you need to customize the generation process
var json = File.ReadAllText("appsettings.json");
var jsonDoc = JsonDocument.Parse(json);
var generator = new AppSettingsGenerator();

// Generate with custom namespace
var code = generator.GenerateClass(jsonDoc.RootElement, "MyCustomNamespace");

// Post-process the generated code if needed
code = code.Replace("public class AppSettings", "public sealed class AppSettings");

File.WriteAllText("CustomAppSettings.cs", code);
```

### Batch Generation
```csharp
// Generate from multiple configuration files
var configFiles = new[]
{
    ("appsettings.json", "AppSettings"),
    ("appsettings.Development.json", "DevelopmentSettings"),
    ("appsettings.Production.json", "ProductionSettings")
};

foreach (var (file, className) in configFiles)
{
    if (File.Exists(file))
    {
        var code = GuardAppSettingsLibrary.GenerateFromFile(file, $"Configuration.{className}");
        File.WriteAllText($"Generated/{className}.cs", code);
    }
}
```

## Error Handling

Always wrap generation calls in try-catch blocks:

```csharp
try
{
    GuardAppSettingsLibrary.GenerateToFile("./appsettings.json", "./AppSettings.cs");
}
catch (FileNotFoundException)
{
    Console.WriteLine("appsettings.json not found");
}
catch (JsonException ex)
{
    Console.WriteLine($"Invalid JSON: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Generation failed: {ex.Message}");
}
```

## Best Practices

1. **Build-time generation**: Use MSBuild integration for automatic generation during build
2. **Source control**: Commit generated files to source control for team consistency
3. **CI/CD**: Regenerate files in your build pipeline to ensure they're up-to-date
4. **Error handling**: Always handle file not found and JSON parsing errors
5. **Namespace consistency**: Use your project's root namespace for generated classes