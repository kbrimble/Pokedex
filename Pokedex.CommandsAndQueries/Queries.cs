using FluentValidation;

namespace Pokedex.CommandsAndQueries;

public record GetPokemonByNameQuery(string Name) : IQuery;

public class GetPokemonByNameQueryValidator : AbstractValidator<GetPokemonByNameQuery>
{
    public GetPokemonByNameQueryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().Matches("[\\w-]+").WithMessage("'{PropertyName}' can only contain letters, numbers and dashes");
    }
}
