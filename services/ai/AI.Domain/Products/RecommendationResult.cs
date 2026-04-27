namespace AI.Domain.Products;

public sealed record RecommendationResult(
    IReadOnlyCollection<ProductRecommendation> Recommendations,
    string Provider,
    string Model
);