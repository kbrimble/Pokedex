using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Pokedex.External.FunTranslations;
using Polly;
using Polly.CircuitBreaker;
using Refit;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy =
        Policy<HttpResponseMessage>
            .HandleResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
            .CircuitBreakerAsync(0, TimeSpan.FromMinutes(10));

    public static IServiceCollection AddFunTranslationsIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FunTranslationsConfiguration>(configuration.GetSection("FunTranslations"));
        services.AddTransient<IFunTranslationService, FunTranslationService>();
        services.AddRefitClient<IFunTranslationsRefitClient>()
            .ConfigureHttpClient((provider, client) =>
            {
                var settings = provider.GetRequiredService<IOptions<FunTranslationsConfiguration>>();
                client.BaseAddress = new Uri(settings.Value.BaseUrl);
            })
            .AddPolicyHandler(_circuitBreakerPolicy);
        return services;
    }
}
