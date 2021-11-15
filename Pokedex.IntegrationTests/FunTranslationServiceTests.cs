using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Pokedex.Domain;
using Pokedex.External.FunTranslations;
using Pokedex.External.Pokeapi;
using Refit;
using Xunit;

namespace Pokedex.IntegrationTests;

public class FunTranslationServiceTests
{
    private readonly FunTranslationService _sut;

    public FunTranslationServiceTests()
    {
        var options = ConfigurationHelpers.GetConfigClass<FunTranslationsConfiguration>("FunTranslations");
        _sut = new FunTranslationService(RestService.For<IFunTranslationsRefitClient>(options.BaseUrl), new NullLogger<FunTranslationService>());
    }

    // Ideally we'd have a test for each translation types but the strict rate limiting
    // from FunTranslations makes this prohibitive
    [SkippableFact]
    public async Task TranslateReturnsValidTranslation()
    {
        const string inputText = "i am yoda";
        const string expectedText = "Yoda,  I am";

        var result = await _sut.Translate(inputText, TranslationType.Yoda);

        // Skip this test if calling the API was rate limited
        Skip.If(result == string.Empty);

        result.Should().Be(expectedText);
    }
}
