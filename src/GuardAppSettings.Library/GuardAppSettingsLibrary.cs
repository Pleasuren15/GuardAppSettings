using System.Text.Json;

namespace GuardAppSettings.Library;

/// <summary>
/// Library API for generating strongly typed configuration classes from JSON
/// </summary>
public static class GuardAppSettingsLibrary
{
    /// <summary>
    /// Generate AppSettings class code from appsettings.json file path
    /// </summary>
    /// <param name="appSettingsPath">Path to appsettings.json file</param>
    /// <returns>Generated C# class code</returns>
    public static string GenerateFromFile(string appSettingsPath)
    {
        if (!File.Exists(appSettingsPath))
            throw new FileNotFoundException($"appsettings.json not found at {appSettingsPath}");

        var json = File.ReadAllText(appSettingsPath);
        return GenerateFromJson(json);
    }

    /// <summary>
    /// Generate AppSettings class code from JSON string
    /// </summary>
    /// <param name="json">JSON configuration string</param>
    /// <returns>Generated C# class code</returns>
    public static string GenerateFromJson(string json)
    {
        var jsonDocument = JsonDocument.Parse(json);
        var generator = new AppSettingsGenerator();
        return generator.GenerateClass(jsonDocument.RootElement);
    }

    /// <summary>
    /// Generate and save AppSettings class to specified file
    /// </summary>
    /// <param name="appSettingsPath">Path to appsettings.json file</param>
    /// <param name="outputPath">Path where AppSettings.cs will be saved</param>
    public static void GenerateToFile(string appSettingsPath, string outputPath)
    {
        var classCode = GenerateFromFile(appSettingsPath);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        File.WriteAllText(outputPath, classCode);
    }

    /// <summary>
    /// Generate AppSettings class with custom namespace
    /// </summary>
    /// <param name="appSettingsPath">Path to appsettings.json file</param>
    /// <param name="namespaceName">Custom namespace for generated class</param>
    /// <returns>Generated C# class code</returns>
    public static string GenerateFromFile(string appSettingsPath, string namespaceName)
    {
        if (!File.Exists(appSettingsPath))
            throw new FileNotFoundException($"appsettings.json not found at {appSettingsPath}");

        var json = File.ReadAllText(appSettingsPath);
        var jsonDocument = JsonDocument.Parse(json);
        var generator = new AppSettingsGenerator();
        return generator.GenerateClass(jsonDocument.RootElement, namespaceName);
    }
}