namespace Catalog.Application.Abstractions.AI;

public sealed record AiProduct(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Category
);