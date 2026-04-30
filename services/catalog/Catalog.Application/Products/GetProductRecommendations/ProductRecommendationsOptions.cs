namespace Catalog.Application.Products.GetProductRecommendations;

public sealed class ProductRecommendationsOptions
{
    public int MaxCandidates { get; set; } = 20;
    public int FallbackCount { get; set; } = 3;
}