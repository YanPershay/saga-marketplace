using System.Text.Json;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Order.Application.Abstractions;
using Order.Domain.Orders;

namespace Order.Application.Orders.CreateOrder;

public sealed class CreateOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderUnitOfWork _orderUnitOfWork;

    public CreateOrderHandler(IOrderRepository orderRepository, IOrderUnitOfWork orderUnitOfWork)
    {
        _orderRepository = orderRepository;
        _orderUnitOfWork = orderUnitOfWork;
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

        var orderCreatedEvent = new OrderCreatedIntegrationEvent(
            order.Id,
            order.CustomerId,
            order.TotalPrice,
            order.CreatedAt,
            order.OrderItems
                .Select(item => new OrderCreatedIntegrationEventItem(
                    item.ProductId,
                    item.Quantity))
                .ToList()
        );

        var envelope = EventEnvelope<OrderCreatedIntegrationEvent>.Create(orderCreatedEvent);

        var payload = JsonSerializer.Serialize(envelope, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await _orderUnitOfWork.AddOrderWithOutboxMessageAsync(
            order,
            envelope.EventType,
            "order.created",
            payload,
            envelope.MessageId,
            envelope.CorrelationId,
            envelope.OccurredAtUtc,
            cancellationToken);

        return order.Id;
    }
}