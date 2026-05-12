using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Microsoft.Extensions.Logging;
using Order.Application.Abstractions;

namespace Order.Application.Saga;

public sealed class ShipmentCreatedHandler
{
    private const string ConsumerName = "order-shipment-created-consumer";

    private readonly IOrderSagaUnitOfWork _unitOfWork;
    private readonly ILogger<ShipmentCreatedHandler> _logger;

    public ShipmentCreatedHandler(
        IOrderSagaUnitOfWork unitOfWork,
        ILogger<ShipmentCreatedHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(
        EventEnvelope<ShipmentCreatedIntegrationEvent> envelope,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed = await _unitOfWork.HasInboxMessageAsync(
            envelope.MessageId,
            ConsumerName,
            cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning(
                "Duplicate ShipmentCreated event detected. MessageId: {MessageId}",
                envelope.MessageId);

            return;
        }

        _logger.LogInformation(
            "Handling ShipmentCreated event for OrderId: {OrderId}",
            envelope.Payload.OrderId);

        await _unitOfWork.HandleShipmentCreatedAsync(
            envelope,
            ConsumerName,
            cancellationToken);

        _logger.LogInformation(
            "ShipmentCreated handled successfully for OrderId: {OrderId}",
            envelope.Payload.OrderId);
    }
}