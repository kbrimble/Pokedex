using System.Text.Json.Serialization;
using Refit;

namespace Pokedex.External.Pokeapi;

public record PokemonIdResponse(int Id, string Name);
public record PokemonDetailsResponse(
    int Id,
    string Name,
    PokemonSpeciesHabitat Habitat,
    bool IsLegendary,
    [property: JsonPropertyName("flavor_text_entries")] List<PokemonFlavourTextEntry> FlavorTextEntries);
public record PokemonSpeciesHabitat(string Name);
public record PokemonFlavourTextEntry(
    [property: JsonPropertyName("flavor_text")] string FlavorText,
    FlavourTextLanguage Language);
public record FlavourTextLanguage(string Name);

public interface IPokeApiRefitClient
{
    [Get("/api/v2/pokemon/{pokemonName}")]
    Task<ApiResponse<PokemonIdResponse>> GetPokemon(string pokemonName);

    [Get("/api/v2/pokemon-species/{pokemonId}")]
    Task<ApiResponse<PokemonDetailsResponse>> GetPokemonDetails(int pokemonId);
}
