using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Pokedex.Api;
using Pokedex.External.Pokeapi;
using Xunit;

namespace Pokedex.ComponentTests;

public class GetPokemonTests
{
    [Fact]
    public async Task GetPokemonReturnsPokemonDetails()
    {
        await using var application = new PokedexApplication();

        var pokeapiService = application.Services.GetRequiredService<IPokeapiService>();
        pokeapiService.GetPokemonId(Arg.Is("bulbasaur")).Returns(new PokemonId(1, "bulbasaur"));
        pokeapiService.GetPokemonDetails(Arg.Is(1)).Returns(new PokemonDetails(1, "bulbasaur", "he is a bulbasaur", "grassland", false));

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

    [Fact]
    public async Task GetPokemonReturnsNotFoundForNonExistentPokemon()
    {
        await using var application = new PokedexApplication();

        var pokeapiService = application.Services.GetRequiredService<IPokeapiService>();
        pokeapiService.GetPokemonId(Arg.Any<string>()).Returns(PokemonId.Empty);

        var client = application.CreateClient();

        var result = await client.GetAsync("/pokemon/does-not-exist");

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
