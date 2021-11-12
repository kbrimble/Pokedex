using FluentValidation;
using Microsoft.Extensions.Logging;
using Pokedex.CommandsAndQueries;
using Pokedex.Domain;
using Pokedex.External.Pokeapi;

namespace Pokedex.Application;

public class GetPokemonByNameQueryHandler : IQueryHandler<GetPokemonByNameQuery, Pokemon>
{
    private readonly IPokeapiService _pokeapiService;
    private readonly IValidator<GetPokemonByNameQuery> _validator;
    private readonly ILogger<GetPokemonByNameQueryHandler> _logger;

    public GetPokemonByNameQueryHandler(IPokeapiService pokeapiService, IValidator<GetPokemonByNameQuery> validator, ILogger<GetPokemonByNameQueryHandler> logger)
    {
        _pokeapiService = pokeapiService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Pokemon> Execute(GetPokemonByNameQuery query)
    {
        await _validator.ValidateAndThrowAsync(query);

        try
        {
            var idResult = await _pokeapiService.GetPokemonId(query.Name);

            if (idResult == PokemonId.Empty)
                return Pokemon.Empty;

            _logger.LogDebug("Found Pokemon with ID {Id} for name {Name}", idResult.Id, query.Name);

            var detailsResult = await _pokeapiService.GetPokemonDetails(idResult.Id);

            if (detailsResult == PokemonDetails.Empty)
                return Pokemon.Empty;

            _logger.LogDebug("Found Pokemon details for ID {Id}", idResult.Id);

            return new Pokemon(idResult.Id, idResult.Name, detailsResult.Description, detailsResult.Habitat, detailsResult.IsLegendary);
        }
        catch (Exception e)
        {
            throw new FailedToGetPokemonException(query.Name, e);
        }
    }
}
