namespace Catalog.Application.Abstractions.AI;

public interface IAiRecommendationClient
{
    Task<IReadOnlyCollection<AiRecommendation>> GetRecommendationsAsync(
        AiProduct product,
        IReadOnlyCollection<AiProduct> candidates,
        CancellationToken cancellationToken);
}