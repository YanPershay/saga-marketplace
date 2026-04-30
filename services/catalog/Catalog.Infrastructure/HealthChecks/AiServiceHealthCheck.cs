using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Catalog.Infrastructure.HealthChecks;

public sealed class AiServiceHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;

    public AiServiceHealthCheck(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.GetAsync("health", cancellationToken);
            
            return response.IsSuccessStatusCode 
                ? HealthCheckResult.Healthy("AI Service is reachable.")
                : HealthCheckResult.Unhealthy($"AI Service returned status code {(int)response.StatusCode}.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("AI Service is unreachable.", ex);
        }
    }
}