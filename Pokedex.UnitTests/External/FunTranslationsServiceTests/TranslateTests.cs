using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Pokedex.Domain;
using Pokedex.External.FunTranslations;
using Xunit;
using static Pokedex.UnitTests.TestHelpers;

namespace Pokedex.UnitTests.External.FunTranslationsServiceTests;

public class TranslateTests
{
    [Theory]
    [InlineData(TranslationType.Shakespeare)]
    [InlineData(TranslationType.Yoda)]
    public async Task TranslationIsReturnedOnSuccess(TranslationType translationType)
    {
        const string translatedText = "some translated text";
        var refitClient = Substitute.For<IFunTranslationsRefitClient>();
        var translationResponse = new TranslationResponse(new TranslationContent(translatedText));
        refitClient.Translate(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>()).Returns(BuildApiResponse(HttpStatusCode.OK, translationResponse));
        var sut = new FunTranslationService(refitClient, new NullLogger<FunTranslationService>());

        var result = await sut.Translate("some input text", translationType);

        result.Should().Be(translatedText);
    }

    [Theory]
    [InlineData(TranslationType.Shakespeare)]
    [InlineData(TranslationType.Yoda)]
    public async Task EmptyStringIsReturnedWhenContextIsNull(TranslationType translationType)
    {
        var refitClient = Substitute.For<IFunTranslationsRefitClient>();
        refitClient.Translate(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>()).Returns(BuildApiResponse<TranslationResponse>(HttpStatusCode.OK, null));
        var sut = new FunTranslationService(refitClient, new NullLogger<FunTranslationService>());

        var result = await sut.Translate("some input text", translationType);

        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    public void ExceptionIsThrownForCommonErrorResponses(HttpStatusCode responseCode)
    {
        var refitClient = Substitute.For<IFunTranslationsRefitClient>();
        refitClient.Translate(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>()).Returns(BuildApiResponse<TranslationResponse>(responseCode, null));
        var sut = new FunTranslationService(refitClient, new NullLogger<FunTranslationService>());

        Func<Task> act = async () => await sut.Translate("some input text", TranslationType.Yoda);

        act.Should().Throw<FailedToTranslateTextException>();
    }

    [Fact]
    public void ExceptionIsThrownWhenFunTranslationsApiFails()
    {
        var refitClient = Substitute.For<IFunTranslationsRefitClient>();
        refitClient.Translate(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>()).Throws<FormatException>();
        var sut = new FunTranslationService(refitClient, new NullLogger<FunTranslationService>());

        Func<Task> act = async () => await sut.Translate("some input text", TranslationType.Yoda);

        act.Should().Throw<FailedToTranslateTextException>().WithInnerException<FormatException>();
    }
}
