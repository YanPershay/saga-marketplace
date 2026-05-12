using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Microsoft.Extensions.Logging;
using Order.Application.Abstractions;

namespace Order.Application.Saga;

public class ShipmentFailedHandler
{
    private const string ConsumerName = "order-shipment-failed-consumer";

    private readonly IOrderSagaUnitOfWork _unitOfWork;
    private readonly ILogger<ShipmentFailedHandler> _logger;

    public ShipmentFailedHandler(
        IOrderSagaUnitOfWork unitOfWork,
        ILogger<ShipmentFailedHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(
        EventEnvelope<ShipmentFailedIntegrationEvent> envelope,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed = await _unitOfWork.HasInboxMessageAsync(
            envelope.MessageId,
            ConsumerName,
            cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning(
                "Duplicate ShipmentFailed event detected. MessageId: {MessageId}",
                envelope.MessageId);

            return;
        }

        _logger.LogInformation(
            "Handling ShipmentFailed event for OrderId: {OrderId}",
            envelope.Payload.OrderId);

        await _unitOfWork.HandleShipmentFailedAsync(
            envelope,
            ConsumerName,
            cancellationToken);

        _logger.LogInformation(
            "ShipmentFailed handled successfully for OrderId: {OrderId}",
            envelope.Payload.OrderId);
    }
}