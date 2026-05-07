using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payment.Application.Abstractions;
using Payment.Application.Options;

namespace Payment.Application.Payments;

public sealed class InventoryReservedHandler
{
    private const string ConsumerName = "payment-inventory-reserved-consumer";

    private readonly IPaymentUnitOfWork _unitOfWork;
    private readonly PaymentSimulationOptions _simulationOptions;
    private readonly ILogger<InventoryReservedHandler> _logger;

    public InventoryReservedHandler(
        IPaymentUnitOfWork unitOfWork,
        IOptions<PaymentSimulationOptions> simulationOptions,
        ILogger<InventoryReservedHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _simulationOptions = simulationOptions.Value;
        _logger = logger;
    }

    public async Task HandleAsync(
        EventEnvelope<InventoryReservedIntegrationEvent> envelope,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed = await _unitOfWork.HasInboxMessageAsync(
            envelope.MessageId, ConsumerName, cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning(
                "Duplicate InventoryReserved message detected. MessageId: {MessageId}",
                envelope.MessageId);

            return;
        }

        _logger.LogInformation(
            "Processing InventoryReserved event. OrderId: {OrderId}",
            envelope.Payload.OrderId);

        var amount = 100m;

        await _unitOfWork.ProcessPaymentAndSaveOutboxAsync(
            envelope,
            ConsumerName,
            _simulationOptions.ForceFailure,
            cancellationToken);

        _logger.LogInformation(
            "Payment processing completed for OrderId: {OrderId}",
            envelope.Payload.OrderId);
    }
}