using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shipping.Application.Abstractions;
using Shipping.Application.Options;

namespace Shipping.Application.Shipments;

public sealed class ShipmentRequestedHandler
{
    private const string ConsumerName = "shipping-shipment-requested-consumer";
    
    private readonly IShippingUnitOfWork _unitOfWork;
    private readonly ShippingSimulationOptions _options;
    private readonly ILogger<ShipmentRequestedHandler> _logger;
    
    public ShipmentRequestedHandler(
        IShippingUnitOfWork unitOfWork,
        IOptions<ShippingSimulationOptions> options,
        ILogger<ShipmentRequestedHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _options = options.Value;
        _logger = logger;
    }

    public async Task HandleAsync(
        EventEnvelope<ShipmentRequestedIntegrationEvent> envelope, CancellationToken cancellationToken = default)
    {
        var alreadyProcessed = await _unitOfWork.HasInboxMessageAsync(
            envelope.MessageId,
            ConsumerName,
            cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning(
                "Duplicate ShipmentRequested message detected. MessageId: {MessageId}",
                envelope.MessageId);

            return;
        }
        
        _logger.LogInformation(
            "Processing ShipmentRequested event. OrderId: {OrderId}",
            envelope.Payload.OrderId);
        
        await _unitOfWork.CreateShipmentAndSaveOutboxAsync(
            envelope,
            ConsumerName,
            _options.ForceFailure,
            cancellationToken);
        
        _logger.LogInformation(
            "Shipment processing completed for OrderId: {OrderId}",
            envelope.Payload.OrderId);
    }
}