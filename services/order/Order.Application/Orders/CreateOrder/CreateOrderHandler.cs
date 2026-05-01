using Order.Application.Abstractions;
using Order.Domain.Orders;

namespace Order.Application.Orders.CreateOrder;

public sealed class CreateOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    
    public CreateOrderHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Guid> HandleAsync(
        CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Items == null || !command.Items.Any())
            throw new ArgumentException("Order must contain at least one item.");

        var orderItems = command.Items
            .Select(item => new OrderItem(
                item.ProductId,
                item.Quantity,
                item.Price
            ))
            .ToList();

        var order = Domain.Orders.Order.Create(command.CustomerId, orderItems);
        
        await  _orderRepository.AddAsync(order, cancellationToken);
        
        return order.Id;
    }
}