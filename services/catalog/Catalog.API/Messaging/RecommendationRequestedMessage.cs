namespace Catalog.API.Messaging;

public sealed record RecommendationRequestedMessage(
    Guid RequestId,
    Guid ProductId,
    ProductContextMessage CurrentProduct,
    IReadOnlyCollection<CandidateProductMessage> CandidateProducts,
    string CorrelationId,
    DateTimeOffset RequestedAtUtc);

public sealed record ProductContextMessage(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Category);

public sealed record CandidateProductMessage(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Category);