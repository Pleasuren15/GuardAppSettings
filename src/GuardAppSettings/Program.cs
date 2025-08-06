using System.CommandLine;
using System.Text.Json;

namespace GuardAppSettings;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var projectOption = new Option<string>(
            "--project",
            description: "Path to the directory containing appsettings.json")
        {
            IsRequired = true
        };

        var targetOption = new Option<string>(
            "--target", 
            description: "Path to the directory where AppSettings.cs will be created")
        {
            IsRequired = true
        };

        var createCommand = new Command("create", "Generate strongly typed AppSettings class from appsettings.json")
        {
            projectOption,
            targetOption
        };

        createCommand.SetHandler(async (string projectPath, string targetPath) =>
        {
            await CreateAppSettingsClass(projectPath, targetPath);
        }, projectOption, targetOption);

        var rootCommand = new RootCommand("GuardAppSettings - Generate strongly typed C# classes from appsettings.json")
        {
            createCommand
        };

        return await rootCommand.InvokeAsync(args);
    }

    static async Task CreateAppSettingsClass(string projectPath, string targetPath)
    {
        try
        {
            var appSettingsPath = Path.Combine(projectPath, "appsettings.json");
            
            if (!File.Exists(appSettingsPath))
            {
                Console.WriteLine($"Error: appsettings.json not found at {appSettingsPath}");
                return;
            }

            var json = await File.ReadAllTextAsync(appSettingsPath);
            var jsonDocument = JsonDocument.Parse(json);
            
            var generator = new AppSettingsGenerator();
            var classCode = generator.GenerateClass(jsonDocument.RootElement);
            
            Directory.CreateDirectory(targetPath);
            var outputPath = Path.Combine(targetPath, "AppSettings.cs");
            await File.WriteAllTextAsync(outputPath, classCode);
            
            Console.WriteLine($"Successfully generated AppSettings.cs at {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
