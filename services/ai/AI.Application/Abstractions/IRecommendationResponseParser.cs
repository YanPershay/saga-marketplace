using AI.Domain.Products;

namespace AI.Application.Abstractions;

public interface IRecommendationResponseParser
{
    IReadOnlyCollection<ProductRecommendation> Parse(string response);
}