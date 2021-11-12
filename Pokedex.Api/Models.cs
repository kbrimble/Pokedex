namespace Pokedex.Api;

public record PokemonResponse(int Id, string Name, string Description, string Habitat, bool IsLegendary)
{
    /// <summary> Id of Pokemon </summary>
    public int Id { get; init; } = Id;

    /// <summary> Name of Pokemon </summary>
    public string Name { get; init; } = Name;

    /// <summary> Description of Pokemon </summary>
    public string Description { get; init; } = Description;

    /// <summary> Pokemon's habitat </summary>
    public string Habitat { get; init; } = Habitat;

    /// <summary> True if Pokemon is a legendary Pokemon </summary>
    public bool IsLegendary { get; init; } = IsLegendary;
}
