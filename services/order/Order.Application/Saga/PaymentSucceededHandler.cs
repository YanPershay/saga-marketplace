using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Microsoft.Extensions.Logging;
using Order.Application.Abstractions;

namespace Order.Application.Saga;

public class PaymentSucceededHandler
{
    private const string ConsumerName = "order-payment-succeeded-consumer";

    private readonly IOrderSagaUnitOfWork _unitOfWork;
    private readonly ILogger<PaymentSucceededHandler> _logger;

    public PaymentSucceededHandler(
        IOrderSagaUnitOfWork unitOfWork,
        ILogger<PaymentSucceededHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(
        EventEnvelope<PaymentSucceededIntegrationEvent> envelope,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed = await _unitOfWork.HasInboxMessageAsync(
            envelope.MessageId,
            ConsumerName,
            cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning(
                "Duplicate PaymentSucceeded event detected. MessageId: {MessageId}",
                envelope.MessageId);

            return;
        }

        _logger.LogInformation(
            "Handling PaymentSucceeded event for OrderId: {OrderId}",
            envelope.Payload.OrderId);

        await _unitOfWork.HandlePaymentSucceededAsync(
            envelope,
            ConsumerName,
            cancellationToken);

        _logger.LogInformation(
            "PaymentSucceeded handled successfully for OrderId: {OrderId}",
            envelope.Payload.OrderId);
    }
}