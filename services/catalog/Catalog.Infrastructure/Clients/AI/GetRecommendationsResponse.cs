namespace Catalog.Infrastructure.Clients.AI;

public sealed class GetRecommendationsResponse
{
    public IReadOnlyCollection<RecommendationItem> Recommendations { get; init; } = [];
}

public sealed class RecommendationItem
{
    public Guid ProductId { get; init; }
    public string Reason { get; init; } = null!;
}