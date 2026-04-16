using System;

namespace Catalog.API.Contracts.Responses;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int QuantityAvailable,
    DateTimeOffset CreatedAt);