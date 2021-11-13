using Refit;

namespace Pokedex.External.FunTranslations;

public record TranslationResponse(TranslationContent? Contents);

public record TranslationContent(string? Translated);

public interface IFunTranslationsRefitClient
{
    [Post("/translate/{translationType}.json")]
    Task<ApiResponse<TranslationResponse>> Translate(string translationType, [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> formRequest);
}
