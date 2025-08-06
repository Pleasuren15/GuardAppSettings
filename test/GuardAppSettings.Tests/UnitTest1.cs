using AwesomeAssertions;
using GuardAppSettings.Tests.example;
using Microsoft.Extensions.Configuration;

namespace GuardAppSettings.Tests;

[TestFixture]
public class Tests
{
    AppSettings _appSettings;
    IConfiguration _configuration;

    [SetUp]
    public void Setup()
    {
        var appSettingsPath = Path.Combine(Helpers.GetSolutionDirectory(), "test", "GuardAppSettings.Tests", "example", "appsettings.json");
        _configuration = new ConfigurationBuilder()
            .AddJsonFile(appSettingsPath)
            .Build();
        _appSettings = new AppSettings(_configuration);
    }

    [Test]
    public void GivenAppSettings_WhenGettingConnectionStrings_ThenShouldReturnExpectedConnectionString()
    {
        // Arrange
        var expectedRedisConnectionString = _configuration.GetConnectionString("Redis");
        var expectedDefaultConnectionConnectionString = _configuration.GetConnectionString("DefaultConnection");

        // Act
        var redisConnectionString = _appSettings.ConnectionStrings.Redis;
        var defaultConnectionString = _appSettings.ConnectionStrings.DefaultConnection;

        // Assert
        expectedRedisConnectionString.Should().Be(redisConnectionString);
        expectedDefaultConnectionConnectionString.Should().Be(defaultConnectionString);
    }

    [Test]
    public void GivenAppSettings_WhenGettingLoggingSettings_ThenShouldReturnExpectedLogLevel()
    {
        // Arrange
        var expectedDefaultLogLevel = _configuration["Logging:LogLevel:Default"];
        var expectedMicrosoftAspNetCoreLogLevel = _configuration["Logging:LogLevel:Microsoft.AspNetCore"];

        // Act
        var defaultLogLevel = _appSettings.Logging.LogLevel.Default;
        var microsoftAspNetCoreLogLevel = _appSettings.Logging.LogLevel.MicrosoftAspNetCore;

        // Assert
        expectedDefaultLogLevel.Should().Be(defaultLogLevel);
        expectedMicrosoftAspNetCoreLogLevel.Should().Be(microsoftAspNetCoreLogLevel);
    }

    [Test]
    public void GivenAppSettings_WhenGettingAppSettingsByKey_ThenShouldReturnExpectedValue()
    {
        // Arrange
        var expectedBaseUrl = _configuration["ApiSettings:BaseUrl"];
        var expectedTimeout = int.Parse(_configuration["ApiSettings:Timeout"]!);
        var expectedRetryCount = int.Parse(_configuration["ApiSettings:RetryCount"]!);
        var expectedEnableCaching = bool.Parse(_configuration["ApiSettings:EnableCaching"]!);

        // Act
        var baseUrl = _appSettings.ApiSettings.BaseUrl;
        var timeOut = _appSettings.ApiSettings.Timeout;
        var retryCount = _appSettings.ApiSettings.RetryCount;
        var enableCaching = _appSettings.ApiSettings.EnableCaching;

        // Assert
        expectedBaseUrl.Should().Be(baseUrl);
        expectedTimeout.Should().Be(timeOut);
        expectedRetryCount.Should().Be(retryCount);
        expectedEnableCaching.Should().Be(enableCaching);
    }

    [Test]
    public void GivenAppSettings_WhenGettingFeatureFlags_ThenShouldReturnExpectedValues()
    {
        // Arrange
        const int ExpectedNumberOfValues = 2;

        // Act
        var featureFlags = _appSettings.FeatureFlags.Length;

        // Assert
        featureFlags.Should().Be(ExpectedNumberOfValues, "because the appsettings.json contains two feature flags");

    }
}
