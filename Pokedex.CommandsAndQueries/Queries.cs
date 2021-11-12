using FluentValidation;

namespace Pokedex.CommandsAndQueries;

public record GetPokemonByNameQuery(string Name) : IQuery;

public class GetPokemonByNameQueryValidator : AbstractValidator<GetPokemonByNameQuery>
{
    public GetPokemonByNameQueryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().Matches("^([a-zA-Z0-9-])+$").WithMessage("'{PropertyName}' can only contain letters, numbers and dashes");
    }
}
