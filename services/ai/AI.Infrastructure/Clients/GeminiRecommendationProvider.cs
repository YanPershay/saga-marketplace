using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AI.Application.Abstractions;
using AI.Domain.Products;
using AI.Infrastructure.Clients.Dtos;
using AI.Infrastructure.Exceptions;
using AI.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace AI.Infrastructure.Clients;

public sealed class GeminiRecommendationProvider : IRecommendationProvider
{
    private readonly IRecommendationPromptBuilder _promptBuilder;
    private readonly HttpClient _httpClient;
    private readonly IRecommendationResponseParser _parser;
    private readonly GeminiOptions _options;
    
    private const string ProviderName = "Gemini";
    
    public GeminiRecommendationProvider(
        IRecommendationPromptBuilder promptBuilder,
        HttpClient httpClient,
        IRecommendationResponseParser parser,
        IOptions<GeminiOptions> options)
    {
        _promptBuilder = promptBuilder;
        _httpClient = httpClient;
        _parser = parser;
        _options = options.Value;
    }
    
    public async Task<RecommendationResult> GetRecommendationsAsync(
        ProductContext currentProduct,
        IReadOnlyCollection<CandidateProduct> candidateProducts,
        CancellationToken cancellationToken)
    {
        var prompt = _promptBuilder.BuildPrompt(currentProduct, candidateProducts);
        
        var request = new GeminiGenerateContentRequest
        {
            Contents = new[]
            {
                new GeminiContentDto
                {
                    Parts = new[]
                    {
                        new GeminiPartDto { Text = prompt }
                    }
                }
            }
        };
        
        var endpoint = $"models/{_options.Model}:generateContent";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(request)
        };
        
        httpRequest.Headers.Add("x-goog-api-key", _options.ApiKey);

        HttpResponseMessage response;

        try
        {
            response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("Gemini request timed out.", ex);
        }

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            throw new AiProviderRateLimitException("Gemini rate limit exceeded. Try again later");
        }
        
        if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            throw new InvalidOperationException("Gemini service is temporarily unavailable. Try again later.");
        }

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Gemini API request failed with status code {response.StatusCode}. Response: {responseBody}");
        }
        
        GeminiGenerateContentResponse? geminiResponse;

        try
        {
            geminiResponse = JsonSerializer.Deserialize<GeminiGenerateContentResponse>(
                responseBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Gemini response is not a valid JSON document.", ex);
        }

        var rawModelText = string.Join(
            string.Empty,
            geminiResponse?.Candidates?
                .SelectMany(candidate => candidate.Content?.Parts ??
                                         Array.Empty<GeminiPartDto>())
                .Where(part => !string.IsNullOrWhiteSpace(part.Text))
                .Select(part => part.Text) ?? Array.Empty<string>()
        );

        if (string.IsNullOrWhiteSpace(rawModelText))
        {
            throw new InvalidOperationException("Gemini response does not contain model text.");
        }

        var recommendations = _parser.Parse(rawModelText);
        
        

        return new RecommendationResult(
            recommendations,
            ProviderName,
            _options.Model
        );

    }
}