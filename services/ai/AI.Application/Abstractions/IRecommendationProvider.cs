using AI.Domain.Products;

namespace AI.Application.Abstractions;

public interface IRecommendationProvider
{
    Task<RecommendationResult> GetRecommendationsAsync(
        ProductContext currentProduct,
        IReadOnlyCollection<CandidateProduct> candidateProducts,
        CancellationToken cancellationToken);
}