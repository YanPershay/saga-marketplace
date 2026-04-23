namespace AI.Domain.Products;

public sealed record RecommendationResult
{
    public IReadOnlyCollection<ProductRecommendation> Recommendations { get; init; } = null!;
    public string Provider { get; init; } = null!;
    public string Model { get; init; } = null!;
}