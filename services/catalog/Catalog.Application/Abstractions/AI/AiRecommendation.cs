namespace Catalog.Application.Abstractions.AI;

public record AiRecommendation(
    Guid ProductId,
    string Reason);