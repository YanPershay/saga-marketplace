

using Newtonsoft.Json;

namespace Recommendation.Functions.Models;

public sealed record RecommendationResult(
    [property: JsonProperty("id")]
    string Id,

    [property: JsonProperty("requestId")]
    string RequestId,

    [property: JsonProperty("productId")]
    string ProductId,

    [property: JsonProperty("recommendations")]
    IReadOnlyCollection<RecommendedProductResult> Recommendations,

    [property: JsonProperty("provider")]
    string Provider,

    [property: JsonProperty("model")]
    string Model,

    [property: JsonProperty("status")]
    string Status,

    [property: JsonProperty("generatedAtUtc")]
    DateTimeOffset GeneratedAtUtc,

    [property: JsonProperty("correlationId")]
    string CorrelationId,

    [property: JsonProperty("errorMessage")]
    string? ErrorMessage);

public sealed record RecommendedProductResult(
    [property: JsonProperty("productId")]
    string ProductId,

    [property: JsonProperty("reason")]
    string Reason);