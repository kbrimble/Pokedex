using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Pokedex.Api;
using Xunit;

namespace Pokedex.FunctionalTests;

public class GetPokemonTests
{
    [Fact]
    public async Task GetPokemonReturnsPokemonDetails()
    {
        await using var application = new PokedexApplication();

        var client = application.CreateClient();

        var result = await client.GetAsync("/pokemon/bulbasaur");

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadFromJsonAsync<PokemonResponse>();
        // Compare each property so that we are not asserting on an exact description
        content!.Id.Should().Be(1);
        content.Name.Should().Be("bulbasaur");
        content.Habitat.Should().Be("grassland");
        content.IsLegendary.Should().BeFalse();
        content.Description.Should().NotBeEmpty();
    }
}
