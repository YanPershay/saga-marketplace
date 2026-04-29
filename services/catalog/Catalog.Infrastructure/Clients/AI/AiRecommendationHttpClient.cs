using System.Net.Http.Json;
using Catalog.Application.Abstractions.AI;

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
            throw new TimeoutException("AI service request timed out.", ex);
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"AI service failed: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<GetRecommendationsResponse>(cancellationToken);

            if (result is null)
                throw new InvalidOperationException("AI response is null");

            return result.Recommendations
                .Select(r => new AiRecommendation(r.ProductId, r.Reason))
                .ToList();
        }
    }
}