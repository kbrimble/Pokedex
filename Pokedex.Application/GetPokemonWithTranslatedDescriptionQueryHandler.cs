using Microsoft.Extensions.Logging;
using Pokedex.CommandsAndQueries;
using Pokedex.Domain;

namespace Pokedex.Application;

public class GetPokemonWithTranslatedDescriptionQueryHandler : IQueryHandler<GetPokemonWithTranslatedDescriptionQuery, Pokemon>
{
    private readonly IQueryHandler<GetPokemonByNameQuery, Pokemon> _getPokemonQuery;
    private readonly IQueryHandler<GetTranslatedTextQuery, string> _getTranslatedTextQuery;
    private readonly ILogger<GetPokemonWithTranslatedDescriptionQueryHandler> _logger;

    public GetPokemonWithTranslatedDescriptionQueryHandler(
        IQueryHandler<GetPokemonByNameQuery, Pokemon> getPokemonQuery,
        IQueryHandler<GetTranslatedTextQuery, string> getTranslatedTextQuery,
        ILogger<GetPokemonWithTranslatedDescriptionQueryHandler> logger)
    {
        _getPokemonQuery = getPokemonQuery;
        _getTranslatedTextQuery = getTranslatedTextQuery;
        _logger = logger;
    }

    public async Task<Pokemon> Execute(GetPokemonWithTranslatedDescriptionQuery query)
    {
        var (name, translationType) = query;
        try
        {
            _logger.LogDebug("Getting Pokemon with name {Name}", name);
            var pokemon = await _getPokemonQuery.Execute(new GetPokemonByNameQuery(name));

            if (pokemon == Pokemon.Empty)
                return Pokemon.Empty;

            _logger.LogDebug("Found Pokemon with name, translating description to type {TranslationType}", translationType);
            var translatedDescription = await _getTranslatedTextQuery.Execute(new GetTranslatedTextQuery(pokemon.Description, translationType));

            var useTranslatedDescription = string.IsNullOrWhiteSpace(translatedDescription);
            _logger.LogDebug("Translated description has length {DescriptionLength}. Use translated description? {UseTranslatedDescription}", translatedDescription.Length, useTranslatedDescription);
            return pokemon with { Description = string.IsNullOrWhiteSpace(translatedDescription) ? pokemon.Description : translatedDescription };
        }
        catch (Exception e)
        {
            throw new FailedToGetPokemonWithTranslatedDescriptionException(name, translationType, e);
        }
    }
}
