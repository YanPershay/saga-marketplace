using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Recommendation.Functions.Messages;

namespace Recommendation.Functions.Clients;

public sealed class AiApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AiApiClient(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GetRecommendationsAsync(
        RecommendationRequestedMessage recommendationRequested,
        CancellationToken cancellationToken = default)
    {
        var baseUrl =
            _configuration["AiApi:BaseUrl"]
            ?? throw new InvalidOperationException(
                "AiApi:BaseUrl is not configured.");

        var request = new
        {
            CurrentProduct = recommendationRequested.CurrentProduct,
            CandidateProducts = recommendationRequested.CandidateProducts
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"{baseUrl}/recommendations",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}