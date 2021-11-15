using Pokedex.Domain;

namespace Pokedex.Application;

public class FailedToGetPokemonException : Exception
{
    public FailedToGetPokemonException(string name, Exception innerException)
        : base(GetMessage(name), innerException) { }

    private static string GetMessage(string name)
        => $"Failed to get Pokemon with name {name}";
}

public class FailedToGetPokemonWithTranslatedDescriptionException : Exception
{
    public FailedToGetPokemonWithTranslatedDescriptionException(string name, Exception innerException)
        : base(GetMessage(name), innerException) { }

    private static string GetMessage(string name)
        => $"Failed to get Pokemon with translated description for name {name}";
}
