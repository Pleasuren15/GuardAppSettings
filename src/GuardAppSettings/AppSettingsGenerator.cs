using System.Text;
using System.Text.Json;

namespace GuardAppSettings;

public class AppSettingsGenerator
{
    public string GenerateClass(JsonElement rootElement)
    {
        var sb = new StringBuilder();
        
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
        
        GenerateProperties(sb, rootElement, "");
        
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    private void GenerateProperties(StringBuilder sb, JsonElement element, string keyPrefix)
    {
        foreach (var property in element.EnumerateObject())
        {
            var propertyName = ToPascalCase(property.Name);
            var configKey = string.IsNullOrEmpty(keyPrefix) ? property.Name : $"{keyPrefix}:{property.Name}";
            
            switch (property.Value.ValueKind)
            {
                case JsonValueKind.String:
                    sb.AppendLine($"    public string {propertyName} => _configuration[\"{configKey}\"] ?? string.Empty;");
                    break;
                    
                case JsonValueKind.Number:
                    if (property.Value.TryGetInt32(out _))
                    {
                        sb.AppendLine($"    public int {propertyName} => int.TryParse(_configuration[\"{configKey}\"], out var value) ? value : 0;");
                    }
                    else if (property.Value.TryGetDouble(out _))
                    {
                        sb.AppendLine($"    public double {propertyName} => double.TryParse(_configuration[\"{configKey}\"], out var value) ? value : 0.0;");
                    }
                    break;
                    
                case JsonValueKind.True:
                case JsonValueKind.False:
                    sb.AppendLine($"    public bool {propertyName} => bool.TryParse(_configuration[\"{configKey}\"], out var value) && value;");
                    break;
                    
                case JsonValueKind.Object:
                    var nestedClassName = propertyName + "Settings";
                    sb.AppendLine($"    public {nestedClassName} {propertyName} => new {nestedClassName}(_configuration);");
                    sb.AppendLine();
                    
                    GenerateNestedClass(sb, property.Value, nestedClassName, configKey);
                    break;
                    
                case JsonValueKind.Array:
                    sb.AppendLine($"    public string[] {propertyName} => _configuration.GetSection(\"{configKey}\").Get<string[]>() ?? Array.Empty<string>();");
                    break;
            }
        }
    }
    
    private void GenerateNestedClass(StringBuilder sb, JsonElement element, string className, string keyPrefix)
    {
        sb.AppendLine($"    public class {className}");
        sb.AppendLine("    {");
        sb.AppendLine("        private readonly IConfiguration _configuration;");
        sb.AppendLine();
        sb.AppendLine($"        public {className}(IConfiguration configuration)");
        sb.AppendLine("        {");
        sb.AppendLine("            _configuration = configuration;");
        sb.AppendLine("        }");
        sb.AppendLine();
        
        GenerateProperties(sb, element, keyPrefix);
        
        sb.AppendLine("    }");
        sb.AppendLine();
    }
    
    private string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
            
        return char.ToUpper(input[0]) + input.Substring(1);
    }
}