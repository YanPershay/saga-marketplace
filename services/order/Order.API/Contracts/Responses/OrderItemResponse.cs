namespace Order.API.Contracts.Responses;

public sealed record OrderItemResponse(
    Guid ProductId,
    int Quantity,
    decimal Price);