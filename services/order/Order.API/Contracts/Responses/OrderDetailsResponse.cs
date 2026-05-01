namespace Order.API.Contracts.Responses;

public sealed record OrderDetailsResponse(
    Guid Id,
    Guid CustomerId,
    string Status,
    decimal TotalPrice,
    DateTimeOffset CreatedAt,
    IReadOnlyCollection<OrderItemResponse> Items);

