namespace AI.API.Contracts.Responses;

public sealed record GetRecommendationsResponse
{
    public IReadOnlyCollection<RecommendationItemResponse> Recommendations { get; init; } = null!;
    public string Provider { get; init; } = null!;
    public string Model { get; init; } = null!;
}