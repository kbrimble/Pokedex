using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pokedex.Application;
using Pokedex.CommandsAndQueries;
using Pokedex.Domain;
using Xunit;

namespace Pokedex.UnitTests.Application;

public class GetPokemonWithTranslatedDescriptionQueryHandlerTests
{
    private readonly IQueryHandler<GetPokemonByNameQuery,Pokemon> _getPokemonQuery;
    private readonly IQueryHandler<GetTranslatedTextQuery,string> _getTranslationQuery;

    public GetPokemonWithTranslatedDescriptionQueryHandlerTests()
    {
        _getPokemonQuery = Substitute.For<IQueryHandler<GetPokemonByNameQuery, Pokemon>>();
        _getTranslationQuery = Substitute.For<IQueryHandler<GetTranslatedTextQuery, string>>();
    }

    [Fact]
    public void IfGettingPokemonFailsThenExceptionIsThrown()
    {
        _getPokemonQuery.Execute(Arg.Any<GetPokemonByNameQuery>()).Throws<DivideByZeroException>();
        var sut = GetSut();

        Func<Task> act = async () => await sut.Execute(new GetPokemonWithTranslatedDescriptionQuery("ditto"));

        act.Should().Throw<FailedToGetPokemonWithTranslatedDescriptionException>().WithInnerException<DivideByZeroException>();
    }

    [Fact]
    public async Task IfGettingPokemonReturnsEmptyThenEmptyIsReturned()
    {
        _getPokemonQuery.Execute(Arg.Any<GetPokemonByNameQuery>()).Returns(Pokemon.Empty);
        var sut = GetSut();

        var result = await sut.Execute(new GetPokemonWithTranslatedDescriptionQuery("ditto"));

        TestHelpers.ShouldBeEquivalent(result, Pokemon.Empty);
    }

    [Fact]
    public void IfGettingTranslationFailsThenExceptionIsThrown()
    {
        _getPokemonQuery.Execute(Arg.Any<GetPokemonByNameQuery>()).Returns(new Pokemon(1, "bulbasaur", "bulba", "grassland", false));
        _getTranslationQuery.Execute(Arg.Any<GetTranslatedTextQuery>()).Throws<SystemException>();
        var sut = GetSut();

        Func<Task> act = async () => await sut.Execute(new GetPokemonWithTranslatedDescriptionQuery("bulbasaur"));

        act.Should().Throw<FailedToGetPokemonWithTranslatedDescriptionException>().WithInnerException<SystemException>();
    }

    [Fact]
    public async Task IfTranslationIsEmptyThenOriginalDescriptionIsUsed()
    {
        var pokemon = new Pokemon(1, "bulbasaur", "bulba", "grassland", false);
        _getPokemonQuery.Execute(Arg.Any<GetPokemonByNameQuery>()).Returns(pokemon);
        _getTranslationQuery.Execute(Arg.Any<GetTranslatedTextQuery>()).Returns(string.Empty);
        var sut = GetSut();

        var result = await sut.Execute(new GetPokemonWithTranslatedDescriptionQuery("bulbasaur"));

        result.Description.Should().Be(pokemon.Description);
    }

    [Fact]
    public async Task IfTranslationIsSuccessfulThenTranslatedDescriptionIsUsed()
    {
        var pokemon = new Pokemon(1, "bulbasaur", "bulba", "grassland", false);
        const string translatedText = "Translated text";
        _getPokemonQuery.Execute(Arg.Any<GetPokemonByNameQuery>()).Returns(pokemon);
        _getTranslationQuery.Execute(Arg.Any<GetTranslatedTextQuery>()).Returns(translatedText);
        var sut = GetSut();

        var result = await sut.Execute(new GetPokemonWithTranslatedDescriptionQuery("bulbasaur"));

        var expected = new Pokemon(pokemon.Id, pokemon.Name, translatedText, pokemon.Habitat, pokemon.IsLegendary);

        TestHelpers.ShouldBeEquivalent(result, expected);
    }

    [Fact]
    public async Task IfPokemonHasCaveHabitatThenYodaTranslationIsUsed()
    {
        var pokemon = new Pokemon(41, "zubat", "The world's most annoying Pokemon", "cave", false);
        _getPokemonQuery.Execute(Arg.Any<GetPokemonByNameQuery>()).Returns(pokemon);
        _getTranslationQuery.Execute(Arg.Any<GetTranslatedTextQuery>()).Returns("Translated text");
        var sut = GetSut();

        await sut.Execute(new GetPokemonWithTranslatedDescriptionQuery("zubat"));

        _getTranslationQuery.Received(1).Execute(Arg.Is<GetTranslatedTextQuery>(query => query.TranslationType == TranslationType.Yoda)).Wait();
    }

    [Fact]
    public async Task IfPokemonIsLegendaryThenYodaTranslationIsUsed()
    {
        var pokemon = new Pokemon(150, "mewtwo", "It was created by a scientist after years of horrific gene splicing and DNA engineering experiments.", "rare", true);
        _getPokemonQuery.Execute(Arg.Any<GetPokemonByNameQuery>()).Returns(pokemon);
        _getTranslationQuery.Execute(Arg.Any<GetTranslatedTextQuery>()).Returns("Translated text");
        var sut = GetSut();

        await sut.Execute(new GetPokemonWithTranslatedDescriptionQuery("mewtwo"));

        _getTranslationQuery.Received(1).Execute(Arg.Is<GetTranslatedTextQuery>(query => query.TranslationType == TranslationType.Yoda)).Wait();
    }

    [Fact]
    public async Task IfPokemonIsNotLegendaryOrCaveDwellingThenShakespeareTranslationIsUsed()
    {
        var pokemon = new Pokemon(1, "bulbasaur", "bulba", "grassland", false);
        _getPokemonQuery.Execute(Arg.Any<GetPokemonByNameQuery>()).Returns(pokemon);
        _getTranslationQuery.Execute(Arg.Any<GetTranslatedTextQuery>()).Returns("Translated text");
        var sut = GetSut();

        await sut.Execute(new GetPokemonWithTranslatedDescriptionQuery("bulbasaur"));

        _getTranslationQuery.Received(1).Execute(Arg.Is<GetTranslatedTextQuery>(query => query.TranslationType == TranslationType.Shakespeare)).Wait();
    }

    private GetPokemonWithTranslatedDescriptionQueryHandler GetSut()
        => new(_getPokemonQuery, _getTranslationQuery, Substitute.For<ILogger<GetPokemonWithTranslatedDescriptionQueryHandler>>());
}
