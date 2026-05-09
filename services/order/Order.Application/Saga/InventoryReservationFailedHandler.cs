using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Microsoft.Extensions.Logging;
using Order.Application.Abstractions;

namespace Order.Application.Saga;

public sealed class InventoryReservationFailedHandler
{
    private const string ConsumerName = "order-inventory-reservation-failed-consumer";

    private readonly IOrderSagaUnitOfWork _unitOfWork;
    private readonly ILogger<InventoryReservationFailedHandler> _logger;

    public InventoryReservationFailedHandler(
        IOrderSagaUnitOfWork unitOfWork,
        ILogger<InventoryReservationFailedHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(
        EventEnvelope<InventoryReservationFailedIntegrationEvent> envelope,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed = await _unitOfWork.HasInboxMessageAsync(
            envelope.MessageId,
            ConsumerName,
            cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning(
                "Duplicate InventoryReservationFailed event detected. MessageId: {MessageId}",
                envelope.MessageId);

            return;
        }

        _logger.LogInformation(
            "Handling InventoryReservationFailed event for OrderId: {OrderId}",
            envelope.Payload.OrderId);

        await _unitOfWork.HandleInventoryReservationFailedAsync(
            envelope,
            ConsumerName,
            cancellationToken);

        _logger.LogInformation(
            "InventoryReservationFailed handled successfully for OrderId: {OrderId}",
            envelope.Payload.OrderId);
    }
}