using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payment.Application.Abstractions;
using Payment.Application.Options;

namespace Payment.Application.Payments;

public sealed class PaymentRequestedHandler
{
    private const string ConsumerName = "payment-payment-requested-consumer";

    private readonly IPaymentUnitOfWork _unitOfWork;
    private readonly PaymentSimulationOptions _simulationOptions;
    private readonly ILogger<PaymentRequestedHandler> _logger;

    public PaymentRequestedHandler(
        IPaymentUnitOfWork unitOfWork,
        IOptions<PaymentSimulationOptions> simulationOptions,
        ILogger<PaymentRequestedHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _simulationOptions = simulationOptions.Value;
        _logger = logger;
    }

    public async Task HandleAsync(
        EventEnvelope<PaymentRequestedIntegrationEvent> envelope,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed = await _unitOfWork.HasInboxMessageAsync(
            envelope.MessageId, ConsumerName, cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning(
                "Duplicate PaymentRequested message detected. MessageId: {MessageId}",
                envelope.MessageId);

            return;
        }

        _logger.LogInformation(
            "Processing PaymentRequested event. OrderId: {OrderId}",
            envelope.Payload.OrderId);

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