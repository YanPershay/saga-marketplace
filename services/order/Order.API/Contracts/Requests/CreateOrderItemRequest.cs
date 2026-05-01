namespace Order.API.Contracts.Requests;

public sealed record CreateOrderItemRequest(
    Guid ProductId,
    int Quantity,
    decimal Price);