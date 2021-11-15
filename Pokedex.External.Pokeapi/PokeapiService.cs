using System.Net;
using Microsoft.Extensions.Logging;

namespace Pokedex.External.Pokeapi;

public interface IPokeapiService
{
    Task<PokemonId> GetPokemonId(string name);
    Task<PokemonDetails> GetPokemonDetails(int id);
}

public class PokeapiService : IPokeapiService
{
    private readonly IPokeApiRefitClient _pokeapiClient;
    private readonly ILogger<PokeapiService> _logger;

    public PokeapiService(IPokeApiRefitClient pokeapiClient, ILogger<PokeapiService> logger)
    {
        _pokeapiClient = pokeapiClient;
        _logger = logger;
    }

    public async Task<PokemonId> GetPokemonId(string name)
    {
        name = name.ToLowerInvariant();
        try
        {
            var response = await _pokeapiClient.GetPokemon(name.ToLowerInvariant());

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogDebug("No Pokemon found for name {Name}", name);
                return PokemonId.Empty;
            }

            if (!response.IsSuccessStatusCode)
                throw new FailedToRetrievePokemonDetailsException(name, response.StatusCode, response.ReasonPhrase);

            _logger.LogDebug("Found Pokemon for {Name}", name);
            return Map(response.Content);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to retrieve Pokemon ID for name {Name}", name);
            throw e switch
            {
                FailedToRetrievePokemonDetailsException => e,
                _ => new FailedToRetrievePokemonDetailsException(name, e)
            };
        }
    }

    public async Task<PokemonDetails> GetPokemonDetails(int id)
    {
        try
        {
            var response = await _pokeapiClient.GetPokemonDetails(id);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogDebug("No Pokemon details found for ID {Id}", id);
                return PokemonDetails.Empty;
            }

            if (!response.IsSuccessStatusCode)
                throw new FailedToRetrievePokemonSpeciesException(id, response.StatusCode, response.ReasonPhrase);

            _logger.LogDebug("Found Pokemon details for {Id}", id);
            return Map(response.Content);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to retrieve Pokemon details for ID {Id}", id);
            throw e switch
            {
                FailedToRetrievePokemonSpeciesException => e,
                _ => new FailedToRetrievePokemonSpeciesException(id, e)
            };
        }
    }

    private static PokemonDetails Map(PokemonDetailsResponse? responseContent)
        => responseContent is null
            ? PokemonDetails.Empty
            : new PokemonDetails(responseContent.Id,
                responseContent.Name,
                Map(responseContent.FlavorTextEntries),
                responseContent.Habitat.Name,
                responseContent.IsLegendary);

    // TODO: filter out escape characters and replace with something nicer.
    private static string Map(IEnumerable<PokemonFlavourTextEntry> flavourTextEntries)
        => flavourTextEntries.FirstOrDefault(ft => ft.Language.Name == "en")?.FlavorText ?? string.Empty;

    private static PokemonId Map(PokemonIdResponse? responseContent)
        => responseContent is null
            ? PokemonId.Empty
            : new PokemonId(responseContent.Id, responseContent.Name);
}
