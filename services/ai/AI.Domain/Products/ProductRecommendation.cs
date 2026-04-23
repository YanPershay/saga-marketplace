namespace AI.Domain.Products;

public sealed record ProductRecommendation
{
    public Guid ProductId { get; init; }
    public string Reason { get; init; } = null!;
}