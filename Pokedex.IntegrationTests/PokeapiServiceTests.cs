using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Pokedex.External.Pokeapi;
using Refit;
using Xunit;

namespace Pokedex.IntegrationTests;

public class PokeapiServiceTests
{
    private readonly PokeapiService _sut;

    public PokeapiServiceTests()
    {
        var options = ConfigurationHelpers.GetConfigClass<PokeapiConfiguration>("Pokeapi");
        _sut = new PokeapiService(RestService.For<IPokeApiRefitClient>(options.BaseUrl), new NullLogger<PokeapiService>());
    }

    [Fact]
    public async Task GetPokemonIdReturnsValidPokemonId()
    {
        var result = await _sut.GetPokemonId("bulbasaur");

        var expected = new PokemonId(1, "bulbasaur");

        result.Should().BeEquivalentTo(expected, opts => opts.ComparingByMembers<PokemonId>());
    }

    [Fact]
    public async Task GetPokemonDetailsReturnsValidDetails()
    {
        var result = await _sut.GetPokemonDetails(1);

        // Compare each property so that we are not asserting on an exact description
        result.Id.Should().Be(1);
        result.Name.Should().Be("bulbasaur");
        result.Habitat.Should().Be("grassland");
        result.IsLegendary.Should().BeFalse();
        result.Description.Should().NotBeEmpty();
    }
}
