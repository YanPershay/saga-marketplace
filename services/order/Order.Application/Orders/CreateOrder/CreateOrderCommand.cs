namespace Order.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand(
    Guid CorrelationId,
    Guid CustomerId,
    IReadOnlyCollection<CreateOrderItem> Items);

public sealed record CreateOrderItem(
    Guid ProductId,
    int Quantity,
    decimal Price);