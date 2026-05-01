namespace Order.API.Contracts.Requests;

public sealed record CreateOrderRequest(
    Guid CustomerId,
    IReadOnlyCollection<CreateOrderItemRequest> Items);
    