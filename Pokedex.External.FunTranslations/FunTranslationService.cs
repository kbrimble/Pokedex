using Microsoft.Extensions.Logging;
using Pokedex.Domain;
using Polly.CircuitBreaker;

namespace Pokedex.External.FunTranslations;

public interface IFunTranslationService
{
    Task<string> Translate(string inputText, TranslationType translationType);
}

public class FunTranslationService : IFunTranslationService
{
    private readonly IFunTranslationsRefitClient _funTranslationsClient;
    private readonly ILogger<FunTranslationService> _logger;

    public FunTranslationService(IFunTranslationsRefitClient funTranslationsClient, ILogger<FunTranslationService> logger)
    {
        _funTranslationsClient = funTranslationsClient;
        _logger = logger;
    }

    public async Task<string> Translate(string inputText, TranslationType translationType)
    {
        try
        {
            var form = new Dictionary<string, object> { ["text"] = inputText };
            var response = await _funTranslationsClient.Translate(translationType.ToString().ToLowerInvariant(), form);

            if (!response.IsSuccessStatusCode)
                throw new FailedToTranslateTextException(translationType, response.StatusCode, response.ReasonPhrase);

            _logger.LogDebug("Returning translation for type {TranslationType}", translationType);
            return response.Content?.Contents?.Translated ?? string.Empty;
        }
        catch (BrokenCircuitException)
        {
            _logger.LogInformation("Cannot translate text to {TranslationType} as the request has been rate limited", translationType);
            return string.Empty;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to translate text to type {TranslationType}", translationType);
            throw e switch
            {
                FailedToTranslateTextException => e,
                _ => new FailedToTranslateTextException(translationType, e)
            };
        }
    }
}
