namespace AI.Domain.Products;

public sealed record ProductContext(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Category);