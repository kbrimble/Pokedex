using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pokedex.External.Pokeapi;
using Xunit;
using static Pokedex.UnitTests.TestHelpers;

namespace Pokedex.UnitTests.External.PokeapiServiceTests;

public class GetPokemonIdTests
{
    [Fact]
    public async Task PokemonIdIsReturnedOnSuccess()
    {
        var refitClient = Substitute.For<IPokeApiRefitClient>();
        var mewtwoResponse = new PokemonIdResponse(150, "mewtwo");
        refitClient.GetPokemon(Arg.Is("mewtwo")).Returns(BuildApiResponse(HttpStatusCode.OK, mewtwoResponse));
        var sut = new PokeapiService(refitClient, new NullLogger<PokeapiService>());

        var result = await sut.GetPokemonId("mewtwo");

        var expected = new PokemonId(150, "mewtwo");

        ShouldBeEquivalent(result, expected);
    }

    [Fact]
    public async Task EmptyPokemonIdIsReturnedWhenPokemonIsNotFound()
    {
        var refitClient = Substitute.For<IPokeApiRefitClient>();
        refitClient.GetPokemon(Arg.Is("mewthree")).Returns(BuildApiResponse<PokemonIdResponse>(HttpStatusCode.NotFound, null));
        var sut = new PokeapiService(refitClient, new NullLogger<PokeapiService>());

        var result = await sut.GetPokemonId("mewthree");

        ShouldBeEquivalent(result, PokemonId.Empty);
    }

    [Fact]
    public async Task EmptyPokemonIdIsReturnedWhenReturnedContentIsNull()
    {
        var refitClient = Substitute.For<IPokeApiRefitClient>();
        refitClient.GetPokemon(Arg.Is("mewthree")).Returns(BuildApiResponse<PokemonIdResponse>(HttpStatusCode.OK, null));
        var sut = new PokeapiService(refitClient, new NullLogger<PokeapiService>());

        var result = await sut.GetPokemonId("mewthree");

        ShouldBeEquivalent(result, PokemonId.Empty);
    }

    [Fact]
    void ExceptionIsThrownWhenPokeapiFails()
    {
        var refitClient = Substitute.For<IPokeApiRefitClient>();
        refitClient.GetPokemon(Arg.Any<string>()).Throws<DivideByZeroException>();
        var sut = new PokeapiService(refitClient, new NullLogger<PokeapiService>());

        Func<Task> act = async () => await sut.GetPokemonId("bulbasaur");

        act.Should().Throw<FailedToRetrievePokemonDetailsException>().WithInnerException<DivideByZeroException>();
    }

    [Fact]
    void ExceptionIsThrownWhenPokeapiReturnsNonSuccess()
    {
        var refitClient = Substitute.For<IPokeApiRefitClient>();
        refitClient.GetPokemon(Arg.Any<string>()).Returns(BuildApiResponse<PokemonIdResponse>(HttpStatusCode.ServiceUnavailable, null));
        var sut = new PokeapiService(refitClient, new NullLogger<PokeapiService>());

        Func<Task> act = async () => await sut.GetPokemonId("charmander");

        act.Should().Throw<FailedToRetrievePokemonDetailsException>();
    }
}
