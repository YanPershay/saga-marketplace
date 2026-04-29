namespace Catalog.Application.Products.GetProductRecommendations;

public record ProductRecommendationResult(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int QuantityAvailable,
    DateTimeOffset CreatedAt,
    string? Reason
);