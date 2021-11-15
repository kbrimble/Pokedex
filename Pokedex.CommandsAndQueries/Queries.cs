using FluentValidation;
using Pokedex.Domain;

namespace Pokedex.CommandsAndQueries;

public record GetPokemonByNameQuery(string Name) : IQuery;

public record GetTranslatedTextQuery(string InputText, TranslationType TranslationType) : IQuery;

public record GetPokemonWithTranslatedDescriptionQuery(string Name) : IQuery;

public class GetPokemonByNameQueryValidator : AbstractValidator<GetPokemonByNameQuery>
{
    public GetPokemonByNameQueryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().Matches("^([a-zA-Z0-9-])+$").WithMessage("'{PropertyName}' can only contain letters, numbers and dashes");
    }
}

public class GetTranslatedTextQueryValidator : AbstractValidator<GetTranslatedTextQuery>
{
    public GetTranslatedTextQueryValidator()
    {
        // There are no requirements around the length of the input text but I think that I would agree
        // a sensible maximum and validate that here.
        RuleFor(x => x.TranslationType).IsInEnum();
    }
}
