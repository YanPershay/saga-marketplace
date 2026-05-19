namespace Catalog.API.Contracts.Responses;

public sealed record RecommendationResultResponse(
    string RequestId,
    string ProductId,
    IReadOnlyCollection<RecommendationItemResponse> Recommendations,
    string Provider,
    string Model,
    string Status,
    DateTimeOffset GeneratedAtUtc,
    string? ErrorMessage = null);

public sealed record RecommendationItemResponse(
    string ProductId,
    string Reason);