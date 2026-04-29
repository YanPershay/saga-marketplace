using Catalog.Application.Abstractions.AI;

namespace Catalog.Infrastructure.Clients.AI;

public sealed class GetRecommendationsRequest
{
    public AiProduct CurrentProduct { get; init; } = null!;
    public IReadOnlyCollection<AiProduct> CandidateProducts { get; init; } = null!;
}