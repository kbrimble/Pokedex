using System.Net;

namespace Pokedex.External.Pokeapi;

public record PokemonId(int Id, string Name)
{
    public static readonly PokemonId Empty = new (default, string.Empty);
}

public record PokemonDetails(int Id, string Name, string Description, string Habitat, bool IsLegendary)
{
    public static readonly PokemonDetails Empty = new(default, string.Empty, string.Empty, string.Empty, default);
}

public class PokeapiConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;
}

public class FailedToRetrievePokemonDetailsException : Exception
{
    public FailedToRetrievePokemonDetailsException(string name, HttpStatusCode statusCode, string? message)
        : base(GetMessage(name, statusCode, message)) { }

    public FailedToRetrievePokemonDetailsException(string name, Exception innerException)
        : base(GetMessage(name), innerException) { }

    private static string GetMessage(string name)
        => $"Failed to retrieve Pokemon with name {name}";
    private static string GetMessage(string name, HttpStatusCode statusCode, string? message)
        => $"{GetMessage(name)}. Received {statusCode} with message \"{message}\"";
}

public class FailedToRetrievePokemonSpeciesException : Exception
{
    public FailedToRetrievePokemonSpeciesException(int id, HttpStatusCode statusCode, string? message)
        : base(GetMessage(id, statusCode, message)) { }

    public FailedToRetrievePokemonSpeciesException(int id, Exception innerException)
        : base(GetMessage(id), innerException) { }

    private static string GetMessage(int id)
        => $"Failed to retrieve Pokemon species with ID {id}";
    private static string GetMessage(int id, HttpStatusCode statusCode, string? message)
        => $"{GetMessage(id)}. Received {statusCode} with message \"{message}\"";
}