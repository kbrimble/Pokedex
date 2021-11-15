using System.Net;
using Pokedex.Domain;

namespace Pokedex.External.FunTranslations;


public class FunTranslationsConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;
}

public class FailedToTranslateTextException : Exception
{
    public FailedToTranslateTextException(TranslationType translationType, HttpStatusCode statusCode, string? message)
        : base(GetMessage(translationType, statusCode, message)) { }

    public FailedToTranslateTextException(TranslationType translationType, Exception innerException)
        : base(GetMessage(translationType), innerException) { }

    private static string GetMessage(TranslationType translationType)
        => $"Failed translate text to type {translationType}";
    private static string GetMessage(TranslationType translationType, HttpStatusCode statusCode, string? message)
        => $"{GetMessage(translationType)}. Received {statusCode} with message \"{message}\"";
}
