# Testing GuardAppSettings Locally

## Prerequisites
- .NET 9.0 SDK installed
- Command line access

## Steps to Test

### 1. Build the Project
```bash
dotnet build .\src\GuardAppSettings\ --configuration Debug
```

### 2. Pack the Tool
```bash
dotnet pack --configuration Debug
```

### 3. Install as Global Tool
```bash
dotnet tool install -g --add-source .\src\GuardAppSettings\bin\Debug\ GuardAppSettings
```

If you get conflicts with existing versions:
```bash
dotnet tool uninstall -g GuardAppSettings
dotnet tool install -g --add-source .\src\GuardAppSettings\bin\Debug\ GuardAppSettings
```

### 4. Test the Tool
Navigate to the example directory and run:
```bash
dotnet gas create --project ./example --target ./output
```

### 5. Check the Generated File
```bash
cat ./output/AppSettings.cs
```

### 6. Alternative: Run Without Installing
If you prefer not to install globally, you can run directly:
```bash
cd /mnt/c/Dev/Learning/GuardAppSettings/src/GuardAppSettings
dotnet run -- create --project ../../example --target ../../output
```

## Expected Output
The tool should create `AppSettings.cs` in the output directory with strongly typed classes based on the example `appsettings.json`.

## Troubleshooting
- If `dotnet gas` command not found, try restarting your terminal
- Check that .NET tools directory is in your PATH
- Verify the tool is installed: `dotnet tool list -g`