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

        Func<Task> act = async () => await sut.Execute(new GetPokemonWithTranslatedDescriptionQuery("ditto", TranslationType.Yoda));

        act.Should().Throw<FailedToGetPokemonWithTranslatedDescriptionException>().WithInnerException<DivideByZeroException>();
    }

    [Fact]
    public async Task IfGettingPokemonReturnsEmptyThenEmptyIsReturned()
    {
        _getPokemonQuery.Execute(Arg.Any<GetPokemonByNameQuery>()).Returns(Pokemon.Empty);
        var sut = GetSut();

        var result = await sut.Execute(new GetPokemonWithTranslatedDescriptionQuery("ditto", TranslationType.Yoda));

        TestHelpers.ShouldBeEquivalent(result, Pokemon.Empty);
    }

    [Fact]
    public void IfGettingTranslationFailsThenExceptionIsThrown()
    {
        _getPokemonQuery.Execute(Arg.Any<GetPokemonByNameQuery>()).Returns(new Pokemon(1, "bulbasaur", "bulba", "grassland", false));
        _getTranslationQuery.Execute(Arg.Any<GetTranslatedTextQuery>()).Throws<SystemException>();
        var sut = GetSut();

        Func<Task> act = async () => await sut.Execute(new GetPokemonWithTranslatedDescriptionQuery("bulbasaur", TranslationType.Yoda));

        act.Should().Throw<FailedToGetPokemonWithTranslatedDescriptionException>().WithInnerException<SystemException>();
    }

    [Fact]
    public async Task IfTranslationIsEmptyThenOriginalDescriptionIsUsed()
    {
        var pokemon = new Pokemon(1, "bulbasaur", "bulba", "grassland", false);
        _getPokemonQuery.Execute(Arg.Any<GetPokemonByNameQuery>()).Returns(pokemon);
        _getTranslationQuery.Execute(Arg.Any<GetTranslatedTextQuery>()).Returns(string.Empty);
        var sut = GetSut();

        var result = await sut.Execute(new GetPokemonWithTranslatedDescriptionQuery("bulbasaur", TranslationType.Yoda));

        result.Description.Should().Be(pokemon.Description);
    }

    [Fact]
    public async Task IfTranslationIsSuccessfulThenPokemonWithTranslatedDescriptionIsReturned()
    {
        var pokemon = new Pokemon(1, "bulbasaur", "bulba", "grassland", false);
        const string translatedText = "translated text";
        _getPokemonQuery.Execute(Arg.Any<GetPokemonByNameQuery>()).Returns(pokemon);
        _getTranslationQuery.Execute(Arg.Any<GetTranslatedTextQuery>()).Returns(translatedText);
        var sut = GetSut();

        var result = await sut.Execute(new GetPokemonWithTranslatedDescriptionQuery("bulbasaur", TranslationType.Yoda));

        var expected = new Pokemon(pokemon.Id, pokemon.Name, translatedText, pokemon.Habitat, pokemon.IsLegendary);

        TestHelpers.ShouldBeEquivalent(result, expected);
    }

    private GetPokemonWithTranslatedDescriptionQueryHandler GetSut()
        => new(_getPokemonQuery, _getTranslationQuery, Substitute.For<ILogger<GetPokemonWithTranslatedDescriptionQueryHandler>>());
}
