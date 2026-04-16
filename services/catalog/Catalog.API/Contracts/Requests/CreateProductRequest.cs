namespace Catalog.API.Contracts.Requests;

public sealed record CreateProductRequest
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required int QuantityAvailable { get; init; }
}