using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pokedex.Application;
using Pokedex.CommandsAndQueries;
using Pokedex.Domain;
using Pokedex.External.Pokeapi;
using Xunit;

namespace Pokedex.UnitTests.Application;

public class GetPokemonByNameQueryHandlerTests
{
    private readonly IPokeapiService _pokeapiService;
    private readonly IValidator<GetPokemonByNameQuery> _validator;

    public GetPokemonByNameQueryHandlerTests()
    {
        _pokeapiService = Substitute.For<IPokeapiService>();
        _validator = Substitute.For<IValidator<GetPokemonByNameQuery>>();
    }

    [Fact]
    public void ExceptionIsThrownIfValidationFails()
    {
        _validator.ValidateAsync(Arg.Any<ValidationContext<GetPokemonByNameQuery>>()).ThrowsForAnyArgs(new ValidationException("Invalid"));
        var sut = GetSut();

        Func<Task> act = async () => await sut.Execute(new GetPokemonByNameQuery("1234"));

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void ExceptionIsThrownIfGettingPokemonIdFails()
    {
        _pokeapiService.GetPokemonId(Arg.Any<string>()).Throws<DivideByZeroException>();
        var sut = GetSut();

        Func<Task> act = async () => await sut.Execute(new GetPokemonByNameQuery("squirtle"));

        act.Should().Throw<FailedToGetPokemonException>().WithInnerException<DivideByZeroException>();
    }

    [Fact]
    public async Task EmptyResultIsReturnedIfIdIsNotFound()
    {
        _pokeapiService.GetPokemonId(Arg.Any<string>()).Returns(PokemonId.Empty);
        var sut = GetSut();

        var result = await sut.Execute(new GetPokemonByNameQuery("weedle"));

        TestHelpers.ShouldBeEquivalent(result, Pokemon.Empty);
    }

    [Fact]
    public void ExceptionIsThrownIfGettingPokemonDetailsFails()
    {
        _pokeapiService.GetPokemonId(Arg.Is("bulbasaur")).Returns(new PokemonId(1, "bulbasaur"));
        _pokeapiService.GetPokemonDetails(Arg.Any<int>()).Throws<BadImageFormatException>();
        var sut = GetSut();

        Func<Task> act = async () => await sut.Execute(new GetPokemonByNameQuery("bulbasaur"));

        act.Should().Throw<FailedToGetPokemonException>().WithInnerException<BadImageFormatException>();
    }

    [Fact]
    public async Task EmptyResultIsReturnedIfDetailsAreNotFound()
    {
        _pokeapiService.GetPokemonId(Arg.Is("bulbasaur")).Returns(new PokemonId(1, "bulbasaur"));
        _pokeapiService.GetPokemonDetails(Arg.Any<int>()).Returns(PokemonDetails.Empty);
        var sut = GetSut();

        var result = await sut.Execute(new GetPokemonByNameQuery("bulbasaur"));

        TestHelpers.ShouldBeEquivalent(result, Pokemon.Empty);
    }

    [Fact]
    public async Task PokemonIsReturnedOnSuccess()
    {
        _pokeapiService.GetPokemonId(Arg.Is("bulbasaur")).Returns(new PokemonId(1, "bulbasaur"));
        _pokeapiService.GetPokemonDetails(Arg.Any<int>()).Returns(new PokemonDetails(1, "bulbasaur", "BULBASAUR can be seen napping in bright sunlight.", "grassland", false));
        var sut = GetSut();

        var result = await sut.Execute(new GetPokemonByNameQuery("bulbasaur"));

        var expected = new Pokemon(1, "bulbasaur", "BULBASAUR can be seen napping in bright sunlight.", "grassland", false);

        TestHelpers.ShouldBeEquivalent(result, expected);
    }

    private GetPokemonByNameQueryHandler GetSut() => new(_pokeapiService, _validator, Substitute.For<ILogger<GetPokemonByNameQueryHandler>>());
}
