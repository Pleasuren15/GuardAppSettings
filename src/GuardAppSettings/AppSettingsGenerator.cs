using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GuardAppSettings;

public class AppSettingsGenerator
{
    public string GenerateClass(JsonElement rootElement)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("using System;");
        sb.AppendLine("using Microsoft.Extensions.Configuration;");
        sb.AppendLine();
        sb.AppendLine("namespace AppSettings;");
        sb.AppendLine();
        sb.AppendLine("public class AppSettings");
        sb.AppendLine("{");
        sb.AppendLine("    private readonly IConfiguration _configuration;");
        sb.AppendLine();
        sb.AppendLine("    public AppSettings(IConfiguration configuration)");
        sb.AppendLine("    {");
        sb.AppendLine("        _configuration = configuration;");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        GenerateProperties(sb, rootElement, "", 1);
        
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    private void GenerateProperties(StringBuilder sb, JsonElement element, string keyPrefix, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 4);
        
        foreach (var property in element.EnumerateObject())
        {
            var propertyName = ToValidPropertyName(property.Name);
            var configKey = string.IsNullOrEmpty(keyPrefix) ? property.Name : $"{keyPrefix}:{property.Name}";
            
            switch (property.Value.ValueKind)
            {
                case JsonValueKind.String:
                    sb.AppendLine($"{indent}public string {propertyName} => _configuration[\"{configKey}\"] ?? string.Empty;");
                    break;
                    
                case JsonValueKind.Number:
                    if (property.Value.TryGetInt32(out _))
                    {
                        sb.AppendLine($"{indent}public int {propertyName} => int.TryParse(_configuration[\"{configKey}\"], out var value) ? value : 0;");
                    }
                    else if (property.Value.TryGetDouble(out _))
                    {
                        sb.AppendLine($"{indent}public double {propertyName} => double.TryParse(_configuration[\"{configKey}\"], out var value) ? value : 0.0;");
                    }
                    break;
                    
                case JsonValueKind.True:
                case JsonValueKind.False:
                    sb.AppendLine($"{indent}public bool {propertyName} => bool.TryParse(_configuration[\"{configKey}\"], out var value) && value;");
                    break;
                    
                case JsonValueKind.Object:
                    var nestedClassName = propertyName + "Settings";
                    sb.AppendLine($"{indent}public {nestedClassName} {propertyName} => new {nestedClassName}(_configuration);");
                    sb.AppendLine();
                    
                    GenerateNestedClass(sb, property.Value, nestedClassName, configKey, indentLevel);
                    break;
                    
                case JsonValueKind.Array:
                    sb.AppendLine($"{indent}public string[] {propertyName} => _configuration.GetSection(\"{configKey}\").Get<string[]>() ?? Array.Empty<string>();");
                    break;
            }
        }
    }
    
    private void GenerateNestedClass(StringBuilder sb, JsonElement element, string className, string keyPrefix, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 4);
        var nextIndent = new string(' ', (indentLevel + 1) * 4);
        
        sb.AppendLine($"{indent}public class {className}");
        sb.AppendLine($"{indent}{{");
        sb.AppendLine($"{nextIndent}private readonly IConfiguration _configuration;");
        sb.AppendLine();
        sb.AppendLine($"{nextIndent}public {className}(IConfiguration configuration)");
        sb.AppendLine($"{nextIndent}{{");
        sb.AppendLine($"{nextIndent}    _configuration = configuration;");
        sb.AppendLine($"{nextIndent}}}");
        sb.AppendLine();
        
        GenerateProperties(sb, element, keyPrefix, indentLevel + 1);
        
        sb.AppendLine($"{indent}}}");
        sb.AppendLine();
    }
    
    private string ToValidPropertyName(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "Property";
            
        // Convert to PascalCase
        var pascalCase = ToPascalCase(input);
        
        // Replace invalid characters with underscores
        pascalCase = Regex.Replace(pascalCase, @"[^a-zA-Z0-9_]", "_");
        
        // Ensure it doesn't start with a number
        if (char.IsDigit(pascalCase[0]))
            pascalCase = "_" + pascalCase;
            
        return pascalCase;
    }
    
    private string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
            
        // Split on common delimiters and capitalize each part
        var parts = input.Split(new char[] { '.', '-', '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var result = new StringBuilder();
        
        foreach (var part in parts)
        {
            if (part.Length > 0)
            {
                result.Append(char.ToUpper(part[0]));
                if (part.Length > 1)
                    result.Append(part.Substring(1));
            }
        }
        
        return result.Length > 0 ? result.ToString() : input;
    }
}