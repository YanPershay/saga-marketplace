namespace Catalog.API.Contracts.Requests;

public sealed record RequestRecommendationsRequest(
    RecommendationProductRequest CurrentProduct,
    IReadOnlyCollection<RecommendationProductRequest> CandidateProducts);

public sealed record RecommendationProductRequest(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Category);