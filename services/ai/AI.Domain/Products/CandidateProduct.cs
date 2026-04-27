namespace AI.Domain.Products;

public sealed record CandidateProduct(Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Category
);