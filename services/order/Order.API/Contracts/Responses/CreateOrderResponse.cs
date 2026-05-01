namespace Order.API.Contracts.Responses;

public sealed record CreateOrderResponse(
    Guid OrderId,
    string Status);

