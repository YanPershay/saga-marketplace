using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Inventory.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Orders;

public sealed class OrderCreatedHandler
{
    private const string ConsumerName = "inventory-order-created-consumer";

    private readonly IInventoryUnitOfWork _unitOfWork;
    private readonly ILogger<OrderCreatedHandler> _logger;

    public OrderCreatedHandler(
        IInventoryUnitOfWork unitOfWork,
        ILogger<OrderCreatedHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(
        EventEnvelope<OrderCreatedIntegrationEvent> envelope,
        CancellationToken cancellationToken)
    {
        var alreadyProcessed = await _unitOfWork.HasInboxMessageAsync(
            envelope.MessageId, ConsumerName, cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning(
                "Duplicate OrderCreated message detected. MessageId: {MessageId}",
                envelope.MessageId);

            return;
        }

        _logger.LogInformation(
            "Processing OrderCreated event. OrderId: {OrderId}",
            envelope.Payload.OrderId);

        await _unitOfWork.ReserveInventoryAndSaveOutboxAsync(
            envelope, ConsumerName, cancellationToken);

        _logger.LogInformation(
            "Inventory processing completed for OrderId: {OrderId}",
            envelope.Payload.OrderId);
    }
}