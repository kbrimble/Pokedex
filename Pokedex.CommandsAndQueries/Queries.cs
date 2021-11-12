using FluentValidation;

namespace Pokedex.CommandsAndQueries;

public record GetPokemonIdQuery(string Name) : IQuery;

public class GetPokemonIdQueryValidator : AbstractValidator<GetPokemonIdQuery>
{
    public GetPokemonIdQueryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().Matches("[\\w-]+").WithMessage("'{PropertyName}' can only contain letters, numbers and dashes");
    }
}


public record GetPokemonDetailsQuery(int Id) : IQuery;

public class GetPokemonDetailsQueryValidator : AbstractValidator<GetPokemonDetailsQuery>
{
    public GetPokemonDetailsQueryValidator()
    {
        // Currently, Pokeapi lists 1118 Pokemon in the total results. This is some very simple
        // validation to ensure that we don't get invalid entries to avoid calling the API
        // unnecessarily. In a production system, there would be other methods to validate the
        // total number of Pokemon in the system such as a call to find out the total in Pokeapi
        // that is then cached for, say, 24hrs
        RuleFor(x => x.Id).GreaterThan(0).LessThanOrEqualTo(1118);
    }
}
