namespace Order.Application.Orders.GetOrderById;

public sealed record OrderDetailsResult(
    Guid Id,
    Guid CustomerId,
    string Status,
    decimal TotalPrice,
    DateTimeOffset CreatedAt,
    IReadOnlyCollection<OrderItemResult> Items);

public sealed record OrderItemResult(
    Guid ProductId,
    int Quantity,
    decimal Price);