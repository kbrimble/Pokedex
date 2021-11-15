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
    public FailedToGetPokemonWithTranslatedDescriptionException(string name, TranslationType translationType, Exception innerException)
        : base(GetMessage(name, translationType), innerException) { }

    private static string GetMessage(string name, TranslationType translationType)
        => $"Failed to get Pokemon with name {name} and description translated to type {translationType}";
}
