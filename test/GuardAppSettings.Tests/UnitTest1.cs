using NUnit.Framework.Constraints;

namespace GuardAppSettings.Tests;

[TestFixture]
public class Tests
{
    AppSettings appSettings;

    [SetUp]
    public void Setup()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();   
        var lol = appSettings.ConnectionStrings.Redis;
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}
