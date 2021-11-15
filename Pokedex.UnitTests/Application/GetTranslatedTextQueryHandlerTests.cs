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
using Pokedex.External.FunTranslations;
using Xunit;

namespace Pokedex.UnitTests.Application;

public class GetTranslatedTextQueryHandlerTests
{
    private readonly IFunTranslationService _translationService;
    private readonly IValidator<GetTranslatedTextQuery> _validator;

    public GetTranslatedTextQueryHandlerTests()
    {
        _translationService = Substitute.For<IFunTranslationService>();
        _validator = Substitute.For<IValidator<GetTranslatedTextQuery>>();
    }

    [Fact]
    public void ExceptionIsThrownIfValidationFails()
    {
        _validator.ValidateAsync(Arg.Any<ValidationContext<GetTranslatedTextQuery>>()).ThrowsForAnyArgs(new ValidationException("Invalid"));
        var sut = GetSut();

        Func<Task> act = async () => await sut.Execute(new GetTranslatedTextQuery("some text", TranslationType.Yoda));

        act.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("           ")]
    public async Task InputTextIsReturnedIfItIsEmptyOrWhitespace(string emptyOrWhitespace)
    {
        var sut = GetSut();

        var result = await sut.Execute(new GetTranslatedTextQuery(emptyOrWhitespace, TranslationType.Yoda));

        result.Should().Be(emptyOrWhitespace);
    }

    [Fact]
    public async Task EmptyStringIsReturnedIfGettingTranslationFails()
    {
        _translationService.Translate(Arg.Any<string>(), Arg.Any<TranslationType>()).Throws<RankException>();
        var sut = GetSut();

        var result = await sut.Execute(new GetTranslatedTextQuery("some text", TranslationType.Yoda));

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task TranslatedTextIsReturnedOnSuccess()
    {
        const string translatedText = "this is translated";
        _translationService.Translate(Arg.Any<string>(), Arg.Any<TranslationType>()).Returns(translatedText);
        var sut = GetSut();

        var result = await sut.Execute(new GetTranslatedTextQuery("some text", TranslationType.Shakespeare));

        result.Should().Be(translatedText);
    }

    private GetTranslatedTextQueryHandler GetSut() => new(_translationService, _validator, Substitute.For<ILogger<GetTranslatedTextQueryHandler>>());
}
