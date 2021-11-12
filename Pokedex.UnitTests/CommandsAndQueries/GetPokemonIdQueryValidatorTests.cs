using FluentValidation.TestHelper;
using Pokedex.CommandsAndQueries;
using Xunit;

namespace Pokedex.UnitTests.CommandsAndQueries;

public class GetPokemonIdQueryValidatorTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ValidationErrorWhenNameIsNullOrEmpty(string? name)
    {
        var sut = new GetPokemonIdQueryValidator();

        var result = sut.TestValidate(new GetPokemonIdQuery(name!));

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("pikachu!")]
    [InlineData("+1234")]
    [InlineData("this has spaces")]
    public void ShouldHaveValidationErrorWhenNameDoesNotOnlyContainsLettersDashesAndNumbers(string name)
    {
        var sut = new GetPokemonIdQueryValidator();

        var result = sut.TestValidate(new GetPokemonIdQuery(name));

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("pikachu")]
    [InlineData("1234")]
    [InlineData("something-with-dashes")]
    [InlineData("a-m1x-of-aLL-3")]
    public void ShouldNotHaveValidationErrorWhenNameOnlyContainsLettersDashesAndNumbers(string name)
    {
        var sut = new GetPokemonIdQueryValidator();

        var result = sut.TestValidate(new GetPokemonIdQuery(name));

        result.ShouldNotHaveAnyValidationErrors();
    }
}
