using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Order.Application.Abstractions;
using Order.Domain.Orders;

namespace Order.Application.Orders.CreateOrder;

public sealed class CreateOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEventPublisher _eventPublisher;

    public CreateOrderHandler(IOrderRepository orderRepository, IEventPublisher eventPublisher)
    {
        _orderRepository = orderRepository;
        _eventPublisher = eventPublisher;
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

        await _orderRepository.AddAsync(order, cancellationToken);

        var orderCreatedEvent = new OrderCreatedIntegrationEvent(
            order.Id,
            order.CustomerId,
            order.TotalPrice,
            order.CreatedAt
        );
        
        await _eventPublisher.PublishAsync(orderCreatedEvent, cancellationToken);

        return order.Id;
    }
}