using FluentValidation;
using Pokedex.CommandsAndQueries;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    // In a production system, I'd add an extension for scanning assemblies for these validators
    // to avoid missing a binding
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddTransient<IValidator<GetPokemonByNameQuery>, GetPokemonByNameQueryValidator>();
        services.AddTransient<IValidator<GetTranslatedTextQuery>, GetTranslatedTextQueryValidator>();
        return services;
    }
}
