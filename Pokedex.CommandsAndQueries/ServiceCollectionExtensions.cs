using FluentValidation;
using Pokedex.CommandsAndQueries;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddTransient<IValidator<GetPokemonByNameQuery>, GetPokemonByNameQueryValidator>();

        return services;
    }
}
