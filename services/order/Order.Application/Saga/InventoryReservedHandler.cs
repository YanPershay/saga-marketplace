using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Microsoft.Extensions.Logging;
using Order.Application.Abstractions;

namespace Order.Application.Saga;

public sealed class InventoryReservedHandler
{
    private const string ConsumerName = "order-inventory-reserved-consumer";

    private readonly IOrderSagaUnitOfWork _unitOfWork;
    private readonly ILogger<InventoryReservedHandler> _logger;

    public InventoryReservedHandler(
        IOrderSagaUnitOfWork unitOfWork,
        ILogger<InventoryReservedHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(
        EventEnvelope<InventoryReservedIntegrationEvent> envelope,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed = await _unitOfWork.HasInboxMessageAsync(
            envelope.MessageId,
            ConsumerName,
            cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning(
                "Duplicate InventoryReserved event detected. MessageId: {MessageId}",
                envelope.MessageId);

            return;
        }

        _logger.LogInformation(
            "Handling InventoryReserved event for OrderId: {OrderId}",
            envelope.Payload.OrderId);

        await _unitOfWork.HandleInventoryReservedAsync(
            envelope,
            ConsumerName,
            cancellationToken);

        _logger.LogInformation(
            "InventoryReserved handled successfully for OrderId: {OrderId}",
            envelope.Payload.OrderId);
    }
}