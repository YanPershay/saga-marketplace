namespace Catalog.API.Contracts.Responses;

public sealed record ProductRecommendationResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int QuantityAvailable,
    DateTimeOffset CreatedAt,
    string? Reason);