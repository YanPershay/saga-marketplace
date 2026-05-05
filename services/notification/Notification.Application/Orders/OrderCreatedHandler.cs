using BuildingBlocks.Messaging.Events;
using Microsoft.Extensions.Logging;

namespace Notification.Application.Orders;

public sealed class OrderCreatedHandler
{
    private readonly ILogger<OrderCreatedHandler> _logger;

    public OrderCreatedHandler(ILogger<OrderCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(
        OrderCreatedIntegrationEvent @event,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "OrderCreated event received. " +
            "OrderId: {OrderId}, " +
            "CustomerId: {CustomerId}, " +
            "TotalAmount: {TotalAmount}, " +
            "CreatedAt: {CreatedAt}",
            @event.OrderId,
            @event.CustomerId,
            @event.TotalAmount,
            @event.CreatedAt);

        return Task.CompletedTask;
    }
}