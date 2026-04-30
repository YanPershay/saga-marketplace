using System.Net.Http.Json;
using Catalog.Application.Abstractions.AI;
using Catalog.Application.Abstractions.AI.Exceptions;

namespace Catalog.Infrastructure.Clients.AI;

public class AiRecommendationHttpClient : IAiRecommendationClient
{
    private readonly HttpClient _httpClient;

    public AiRecommendationHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<AiRecommendation>> GetRecommendationsAsync(
        AiProduct product,
        IReadOnlyCollection<AiProduct> candidates,
        CancellationToken cancellationToken)
    {
        var request = new GetRecommendationsRequest
        {
            CurrentProduct = product,
            CandidateProducts = candidates
        };

        HttpResponseMessage response;

        try
        {
            response = await _httpClient.PostAsJsonAsync(
                "recommendations",
                request,
                cancellationToken);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new AiServiceTimeoutException("AI service request timed out.", ex);
        }

        using (response)
        {
            if ((int)response.StatusCode >= 500)
            {
                throw new AiServiceUnavailableException(
                    $"AI service returned server error {(int)response.StatusCode}.");
            }

            if ((int)response.StatusCode >= 400)
            {
                throw new AiServiceBadResponseException(
                    $"AI service returned client error {(int)response.StatusCode}.");
            }

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"AI service failed: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<GetRecommendationsResponse>(cancellationToken);

            if (result is null)
            {
                throw new AiServiceBadResponseException("AI service returned empty response.");
            }

            return result.Recommendations
                .Select(r => new AiRecommendation(r.ProductId, r.Reason))
                .ToList();
        }
    }
}