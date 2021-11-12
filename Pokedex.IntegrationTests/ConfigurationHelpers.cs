using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Pokedex.IntegrationTests;

public static class ConfigurationHelpers
{
    private static IConfigurationRoot GetConfigRoot()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .Build();
    }

    public static T GetConfigClass<T>(string section) where T : class, new()
    {
        var configuration = new T();

        var iConfig = GetConfigRoot();

        iConfig
            .GetSection(section)
            .Bind(configuration);

        return configuration;
    }

    public static IOptions<T> GetOptionsFor<T>(string section) where T : class, new()
    {
        var options = Substitute.For<IOptions<T>>();
        var config = GetConfigClass<T>(section);
        options.Value.Returns(config);
        return options;
    }
}
