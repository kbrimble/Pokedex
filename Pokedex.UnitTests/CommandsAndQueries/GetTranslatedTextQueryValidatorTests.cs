using FluentValidation.TestHelper;
using Pokedex.CommandsAndQueries;
using Pokedex.Domain;
using Xunit;

namespace Pokedex.UnitTests.CommandsAndQueries;

public class GetTranslatedTextQueryValidatorTests
{
    [Fact]
    public void ShouldHaveValidationErrorWhenTranslationTypeIsNotValid()
    {
        var sut = new GetTranslatedTextQueryValidator();

        var result = sut.TestValidate(new GetTranslatedTextQuery("some text", (TranslationType)200));

        result.ShouldHaveValidationErrorFor(x => x.TranslationType);
    }
}
