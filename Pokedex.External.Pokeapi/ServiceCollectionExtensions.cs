using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Pokedex.External.Pokeapi;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;
using Refit;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
        .WaitAndRetryAsync<HttpResponseMessage>(RetryAttempts, (Func<int, TimeSpan>)(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2.0, retryAttempt))));
    private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy = HttpPolicyExtensions.HandleTransientHttpError()
        .CircuitBreakerAsync<HttpResponseMessage>(RetryAttempts, TimeSpan.FromSeconds(10.0));

    private const int RetryAttempts = 3;

    public static IServiceCollection AddPokeapiIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PokeapiConfiguration>(configuration.GetSection("Pokeapi"));
        services.AddTransient<IPokeapiService, PokeapiService>();
        services.AddRefitClient<IPokeApiRefitClient>()
            .ConfigureHttpClient((provider, client) =>
            {
                var settings = provider.GetRequiredService<IOptions<PokeapiConfiguration>>();
                client.BaseAddress = new Uri(settings.Value.BaseUrl);
            })
            .AddPolicyHandler(RetryPolicy.WrapAsync(CircuitBreakerPolicy));
        return services;
    }
}