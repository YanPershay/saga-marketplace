using AI.Domain.Products;

namespace AI.Application.GetRecommendations;

public class GetRecommendationsCommand
{
    public ProductContext CurrentProduct { get; init; } = null!;
    public IReadOnlyCollection<CandidateProduct> CandidateProducts { get; init; } = null!;
}