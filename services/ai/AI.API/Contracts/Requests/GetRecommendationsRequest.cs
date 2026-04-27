namespace AI.API.Contracts.Requests;

public sealed record GetRecommendationsRequest
{
    public IReadOnlyCollection<CandidateProductRequest> CandidateProducts { get; init; } = [];
    public ProductContextRequest CurrentProduct { get; init; } = null!;
}