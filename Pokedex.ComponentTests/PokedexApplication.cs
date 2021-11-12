using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Pokedex.External.Pokeapi;

namespace Pokedex.ComponentTests;

public class PokedexApplication : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.Stub<IPokeapiService>();
        });
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Stub<T>(this IServiceCollection services) where T : class => services
        .RemoveAll(typeof(T))
        .Replace(new ServiceDescriptor(typeof(T), Substitute.For<T>()));
}
