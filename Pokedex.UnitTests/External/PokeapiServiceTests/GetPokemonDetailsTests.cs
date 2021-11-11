using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pokedex.External.Pokeapi;
using Refit;
using Xunit;

namespace Pokedex.UnitTests.External.PokeapiServiceTests;

public class GetPokemonDetailsTests
{
    [Fact]
    public async Task PokemonDetailsIsReturnedOnSuccess()
    {
        var refitClient = Substitute.For<IPokeApiRefitClient>();
        var mewtwoResponse = new PokemonDetailsResponseBuilder(150, "mewtwo", "rare", true,
            "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.").Build();
        refitClient.GetPokemonDetails(Arg.Any<int>()).Returns(BuildApiResponse(HttpStatusCode.OK, mewtwoResponse));
        var sut = new PokeapiService(refitClient, new NullLogger<PokeapiService>());

        var result = await sut.GetPokemonDetails(150);

        var expected = new PokemonDetails(150, "mewtwo", "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.", "rare", true);

        ShouldBeEquivalent(result, expected);
    }

    [Fact]
    public async Task EmptyPokemonDetailsIsReturnedWhenPokemonIsNotFound()
    {
        var refitClient = Substitute.For<IPokeApiRefitClient>();
        refitClient.GetPokemonDetails(Arg.Any<int>()).Returns(BuildApiResponse(HttpStatusCode.NotFound, null));
        var sut = new PokeapiService(refitClient, new NullLogger<PokeapiService>());

        var result = await sut.GetPokemonDetails(1000);

        ShouldBeEquivalent(result, PokemonDetails.Empty);
    }

    [Fact]
    public void ExceptionIsThrownWhenPokeapiFails()
    {
        var refitClient = Substitute.For<IPokeApiRefitClient>();
        refitClient.GetPokemonDetails(Arg.Any<int>()).Throws<DivideByZeroException>();
        var sut = new PokeapiService(refitClient, new NullLogger<PokeapiService>());

        Func<Task> act = async () => await sut.GetPokemonDetails(2);

        act.Should().Throw<FailedToRetrievePokemonSpeciesException>().WithInnerException<DivideByZeroException>();
    }

    [Fact]
    public void ExceptionIsThrownWhenPokeapiReturnsNonSuccess()
    {
        var refitClient = Substitute.For<IPokeApiRefitClient>();
        refitClient.GetPokemonDetails(Arg.Any<int>()).Returns(BuildApiResponse(HttpStatusCode.ServiceUnavailable, null));
        var sut = new PokeapiService(refitClient, new NullLogger<PokeapiService>());

        Func<Task> act = async () => await sut.GetPokemonDetails(1);

        act.Should().Throw<FailedToRetrievePokemonSpeciesException>();
    }

    [Fact]
    public async Task EmptyDescriptionIsReturnedWhenNoFlavourTextExists()
    {
        var refitClient = Substitute.For<IPokeApiRefitClient>();
        var mewtwoResponse = new PokemonDetailsResponseBuilder(150, "mewtwo", "rare", true).Build();
        refitClient.GetPokemonDetails(Arg.Any<int>()).Returns(BuildApiResponse(HttpStatusCode.OK, mewtwoResponse));
        var sut = new PokeapiService(refitClient, new NullLogger<PokeapiService>());

        var result = await sut.GetPokemonDetails(150);

        var expected = new PokemonDetails(150, "mewtwo", "", "rare", true);

        ShouldBeEquivalent(result, expected);
    }

    [Fact]
    public async Task EmptyDescriptionIsReturnedWhenNoEnglishFlavourTextExists()
    {
        var refitClient = Substitute.For<IPokeApiRefitClient>();
        var mewtwoResponse = new PokemonDetailsResponseBuilder(150, "mewtwo", "rare", true)
            .WithFlavourText("Il est le fruit de nombreuses expériences génétiques\nhorribles et malsaines.", "fr").Build();
        refitClient.GetPokemonDetails(Arg.Any<int>()).Returns(BuildApiResponse(HttpStatusCode.OK, mewtwoResponse));
        var sut = new PokeapiService(refitClient, new NullLogger<PokeapiService>());

        var result = await sut.GetPokemonDetails(150);

        var expected = new PokemonDetails(150, "mewtwo", "", "rare", true);

        ShouldBeEquivalent(result, expected);
    }

    [Fact]
    public async Task FirstEntryIsUsedWhenMultipleFlavourTextsAreReturnedForEnglish()
    {
        var refitClient = Substitute.For<IPokeApiRefitClient>();
        const string flavourText1 = "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.";
        const string flavourText2 = "A Pokémon created by recombining\nMew’s genes. It’s said to have the\nmost savage heart among Pokémon.";
        var mewtwoResponse = new PokemonDetailsResponseBuilder(150, "mewtwo", "rare", true)
            .WithFlavourText(flavourText1)
            .WithFlavourText(flavourText2)
            .Build();
        refitClient.GetPokemonDetails(Arg.Any<int>()).Returns(BuildApiResponse(HttpStatusCode.OK, mewtwoResponse));
        var sut = new PokeapiService(refitClient, new NullLogger<PokeapiService>());

        var result = await sut.GetPokemonDetails(150);

        var expected = new PokemonDetails(150, "mewtwo", flavourText1, "rare", true);

        ShouldBeEquivalent(result, expected);
    }

    private static void ShouldBeEquivalent(PokemonDetails actual, PokemonDetails expected)
        => actual.Should().BeEquivalentTo(expected, opts => opts.ComparingByMembers<PokemonDetails>());

    private static ApiResponse<PokemonDetailsResponse> BuildApiResponse(HttpStatusCode httpStatusCode, PokemonDetailsResponse? detailsResponse)
        => new(new HttpResponseMessage(httpStatusCode), detailsResponse!, new RefitSettings());

    private class PokemonDetailsResponseBuilder
    {
        private readonly int _id;
        private readonly string _name;
        private readonly string _habitat;
        private readonly bool _isLegendary;
        private readonly List<PokemonFlavourTextEntry> _flavourTextEntries = new();

        public PokemonDetailsResponseBuilder(int id, string name, string habitat, bool isLegendary, string? englishFlavourText = null)
        {
            _id = id;
            _name = name;
            _habitat = habitat;
            _isLegendary = isLegendary;
            if (englishFlavourText is not null)
                _flavourTextEntries.Add(new PokemonFlavourTextEntry(englishFlavourText, new FlavourTextLanguage("en")));
        }

        public PokemonDetailsResponseBuilder WithFlavourText(string flavourText, string language = "en")
        {
            _flavourTextEntries.Add(new PokemonFlavourTextEntry(flavourText, new FlavourTextLanguage(language)));
            return this;
        }

        public PokemonDetailsResponse Build() => new(_id, _name, new PokemonSpeciesHabitat(_habitat), _isLegendary, _flavourTextEntries);
    }
}