namespace Pokedex.Domain;

public record Pokemon(int Id, string Name, string Description, string Habitat, bool IsLegendary)
{
    public static Pokemon Empty = new(default, string.Empty, string.Empty, string.Empty, default);
}
