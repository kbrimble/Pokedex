using Pokedex.Application;
using Pokedex.CommandsAndQueries;
using Pokedex.Domain;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    // In a production system, I'd add an extension for scanning assemblies for these handlers
    // to avoid missing a binding
    public static IServiceCollection AddCommandAndQueryHandlers(this IServiceCollection services)
    {
        services.AddTransient<IQueryHandler<GetPokemonByNameQuery, Pokemon>, GetPokemonByNameQueryHandler>();
        services.AddTransient<IQueryHandler<GetTranslatedTextQuery, string>, GetTranslatedTextQueryHandler>();
        services.AddTransient<IQueryHandler<GetPokemonWithTranslatedDescriptionQuery, Pokemon>, GetPokemonWithTranslatedDescriptionQueryHandler>();
        return services;
    }
}
