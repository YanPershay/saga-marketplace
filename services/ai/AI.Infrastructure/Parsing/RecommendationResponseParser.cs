using System.Text.Json;
using AI.Application.Abstractions;
using AI.Domain.Products;
using AI.Infrastructure.Parsing.Dtos;

namespace AI.Infrastructure.Parsing;

public sealed class RecommendationResponseParser : IRecommendationResponseParser
{
    public IReadOnlyCollection<ProductRecommendation> Parse(string rawResponse)
    {
        if (string.IsNullOrWhiteSpace(rawResponse))
            throw new ArgumentException("Response cannot be null or empty.", nameof(rawResponse));

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        RecommendationResponseDto? dto;

        try
        {
            dto = JsonSerializer.Deserialize<RecommendationResponseDto>(rawResponse, jsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Recommendation response is not a valid JSON document.", ex);
        }

        if (dto?.Recommendations is null)
            throw new InvalidOperationException("Recommendation response does not contain a valid recommendations collection.");

        if (dto.Recommendations.Count == 0)
            return Array.Empty<ProductRecommendation>();

        var seenProductIds = new HashSet<Guid>();
        var result = new List<ProductRecommendation>();

        foreach (var item in dto.Recommendations)
        {
            if (item is null)
                throw new InvalidOperationException("Recommendation item cannot be null.");

            if (string.IsNullOrWhiteSpace(item.ProductId))
                throw new InvalidOperationException("Recommendation item must contain a non-empty productId.");

            if (!Guid.TryParse(item.ProductId, out var productId))
                throw new InvalidOperationException($"Recommendation item contains invalid productId: '{item.ProductId}'.");

            if (string.IsNullOrWhiteSpace(item.Reason))
                throw new InvalidOperationException($"Recommendation item for productId '{item.ProductId}' must contain a non-empty reason.");

            if (!seenProductIds.Add(productId))
                continue;

            result.Add(new ProductRecommendation(productId, item.Reason));
        }

        return result;
    }
}