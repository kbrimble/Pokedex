using Pokedex.Application;
using Pokedex.CommandsAndQueries;
using Pokedex.Domain;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandAndQueryHandlers(this IServiceCollection services)
    {
        services.AddTransient<IQueryHandler<GetPokemonByNameQuery, Pokemon>, GetPokemonByNameQueryHandler>();
        return services;
    }
}
