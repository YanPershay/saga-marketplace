using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Microsoft.Extensions.Logging;
using Order.Application.Abstractions;

namespace Order.Application.Saga;

public class PaymentFailedHandler
{
    private const string ConsumerName = "order-payment-failed-consumer";

    private readonly IOrderSagaUnitOfWork _unitOfWork;
    private readonly ILogger<PaymentFailedHandler> _logger;

    public PaymentFailedHandler(
        IOrderSagaUnitOfWork unitOfWork,
        ILogger<PaymentFailedHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(
        EventEnvelope<PaymentFailedIntegrationEvent> envelope,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed = await _unitOfWork.HasInboxMessageAsync(
            envelope.MessageId,
            ConsumerName,
            cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning(
                "Duplicate PaymentFailed event detected. MessageId: {MessageId}",
                envelope.MessageId);

            return;
        }

        _logger.LogInformation(
            "Handling PaymentFailed event for OrderId: {OrderId}",
            envelope.Payload.OrderId);

        await _unitOfWork.HandlePaymentFailedAsync(
            envelope,
            ConsumerName,
            cancellationToken);

        _logger.LogInformation(
            "PaymentFailed handled successfully for OrderId: {OrderId}",
            envelope.Payload.OrderId);
    }
}