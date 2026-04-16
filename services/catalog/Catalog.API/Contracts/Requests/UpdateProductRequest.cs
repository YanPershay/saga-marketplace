using System;

namespace Catalog.API.Contracts.Requests;

public sealed record UpdateProductRequest
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required int QuantityAvailable { get; init; }
}