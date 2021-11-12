using FluentValidation.TestHelper;
using Pokedex.CommandsAndQueries;
using Xunit;

namespace Pokedex.UnitTests.CommandsAndQueries;

public class GetPokemonDetailQueryValidatorTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void ShouldHaveValidationErrorForNonPositiveIds(int id)
    {
        var sut = new GetPokemonDetailsQueryValidator();

        var result = sut.TestValidate(new GetPokemonDetailsQuery(id));

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(1119)]
    [InlineData(432432432)]
    public void ShouldHaveValidationErrorForIdsGreaterThanMaximum(int id)
    {
        var sut = new GetPokemonDetailsQueryValidator();

        var result = sut.TestValidate(new GetPokemonDetailsQuery(id));

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(1118)]
    [InlineData(1)]
    [InlineData(150)]
    public void ShouldNotHaveValidationErrorForValidIds(int id)
    {
        var sut = new GetPokemonDetailsQueryValidator();

        var result = sut.TestValidate(new GetPokemonDetailsQuery(id));

        result.ShouldNotHaveAnyValidationErrors();
    }
}