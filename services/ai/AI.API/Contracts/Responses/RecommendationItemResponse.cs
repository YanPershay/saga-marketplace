namespace AI.API.Contracts.Responses;

public sealed record RecommendationItemResponse(Guid ProductId,
    string Reason);