using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pokedex.Api;
using Pokedex.Domain;
using Pokedex.External.FunTranslations;
using Pokedex.External.Pokeapi;
using Xunit;

namespace Pokedex.ComponentTests;

public class GetTranslatedPokemonTests
{
    [Fact]
    public async Task GetPokemonReturnsPokemonDetailsWithTranslatedDescription()
    {
        await using var application = new PokedexApplication();

        var pokeapiService = application.Services.GetRequiredService<IPokeapiService>();
        pokeapiService.GetPokemonId(Arg.Is("bulbasaur")).Returns(new PokemonId(1, "bulbasaur"));
        pokeapiService.GetPokemonDetails(Arg.Is(1)).Returns(new PokemonDetails(1, "bulbasaur", "he is a bulbasaur", "grassland", false));

        var translationService = application.Services.GetRequiredService<IFunTranslationService>();
        translationService.Translate(Arg.Any<string>(), Arg.Any<TranslationType>()).Returns("translated description");

        var client = application.CreateClient();

        var result = await client.GetAsync("/pokemon/translated/bulbasaur");

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadFromJsonAsync<PokemonResponse>();

        var expected = new PokemonResponse(1, "bulbasaur", "translated description", "grassland", false);

        content.Should().BeEquivalentTo(expected, opts => opts.ComparingByMembers<PokemonResponse>());
    }

    [Fact]
    public async Task GetPokemonReturnsPokemonDetailsWithoutTranslatedDescriptionIfGettingTranslationFails()
    {
        await using var application = new PokedexApplication();

        var pokeapiService = application.Services.GetRequiredService<IPokeapiService>();
        pokeapiService.GetPokemonId(Arg.Is("bulbasaur")).Returns(new PokemonId(1, "bulbasaur"));
        pokeapiService.GetPokemonDetails(Arg.Is(1)).Returns(new PokemonDetails(1, "bulbasaur", "he is a bulbasaur", "grassland", false));

        var translationService = application.Services.GetRequiredService<IFunTranslationService>();
        translationService.Translate(Arg.Any<string>(), Arg.Any<TranslationType>()).Returns("");

        var client = application.CreateClient();

        var result = await client.GetAsync("/pokemon/translated/bulbasaur");

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await result.Content.ReadFromJsonAsync<PokemonResponse>();

        var expected = new PokemonResponse(1, "bulbasaur", "he is a bulbasaur", "grassland", false);

        content.Should().BeEquivalentTo(expected, opts => opts.ComparingByMembers<PokemonResponse>());
    }

    [Fact]
    public async Task GetPokemonReturnsNotFoundForNonExistentPokemon()
    {
        await using var application = new PokedexApplication();

        var pokeapiService = application.Services.GetRequiredService<IPokeapiService>();
        pokeapiService.GetPokemonId(Arg.Any<string>()).Returns(PokemonId.Empty);

        var client = application.CreateClient();

        var result = await client.GetAsync("/pokemon/translated/does-not-exist");

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPokemonReturnsInternalServerErrorWhenGettingPokemonFails()
    {
        await using var application = new PokedexApplication();

        var pokeapiService = application.Services.GetRequiredService<IPokeapiService>();
        pokeapiService.GetPokemonId(Arg.Any<string>()).Throws<TimeoutException>();

        var client = application.CreateClient();

        var result = await client.GetAsync("/pokemon/translated/pikachu");

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetPokemonReturnsBadRequestWhenGettingPokemonWithInvalidName()
    {
        await using var application = new PokedexApplication();

        var pokeapiService = application.Services.GetRequiredService<IPokeapiService>();
        pokeapiService.GetPokemonId(Arg.Any<string>()).Throws<TimeoutException>();

        var client = application.CreateClient();

        var result = await client.GetAsync("/pokemon/translated/pika chu");

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
