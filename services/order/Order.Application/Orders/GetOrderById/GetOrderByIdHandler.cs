using Order.Application.Abstractions;

namespace Order.Application.Orders.GetOrderById;

public sealed class GetOrderByIdHandler
{
    private readonly IOrderRepository _orderRepository;
    
    public GetOrderByIdHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<OrderDetailsResult?> HandleAsync(
        GetOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(query.OrderId, cancellationToken);

        if (order is null)
            return null;

        return new OrderDetailsResult(
            order.Id,
            order.CustomerId,
            order.Status.ToString(),
            order.TotalPrice,
            order.CreatedAt,
            order.OrderItems
                .Select(item => new OrderItemResult(
                    item.ProductId,
                    item.Quantity,
                    item.Price))
                .ToList());
    }
}