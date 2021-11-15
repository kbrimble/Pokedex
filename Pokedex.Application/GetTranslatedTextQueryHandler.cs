using FluentValidation;
using Microsoft.Extensions.Logging;
using Pokedex.CommandsAndQueries;
using Pokedex.External.FunTranslations;

namespace Pokedex.Application;

public class GetTranslatedTextQueryHandler : IQueryHandler<GetTranslatedTextQuery, string>
{
    private readonly IFunTranslationService _translationService;
    private readonly IValidator<GetTranslatedTextQuery> _validator;
    private readonly ILogger<GetTranslatedTextQueryHandler> _logger;

    public GetTranslatedTextQueryHandler(IFunTranslationService translationService, IValidator<GetTranslatedTextQuery> validator, ILogger<GetTranslatedTextQueryHandler> logger)
    {
        _translationService = translationService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<string> Execute(GetTranslatedTextQuery query)
    {
        // This is a workaround to throw on validation error but also make this method testable.
        // The solution here is to wrap validation in another service and to mock that but I've
        // left that out here.
        await _validator.ValidateAsync(query, options => options.ThrowOnFailures());

        try
        {
            if (string.IsNullOrWhiteSpace(query.InputText))
                return query.InputText;

            return await _translationService.Translate(query.InputText, query.TranslationType);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get translation of type {TranslationType}", query.TranslationType);
            _logger.LogDebug("Text that failed to translate: \"{InputText}\"", query.InputText);
            return string.Empty;
        }
    }
}
