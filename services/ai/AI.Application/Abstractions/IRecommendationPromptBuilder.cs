using AI.Domain.Products;

namespace AI.Application.Abstractions;

public interface IRecommendationPromptBuilder
{
    string BuildPrompt(ProductContext currentProduct, IReadOnlyCollection<CandidateProduct> candidateProducts);
}