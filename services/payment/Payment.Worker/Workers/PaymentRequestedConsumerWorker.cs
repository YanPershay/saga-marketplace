using Payment.Infrastructure.Messaging;

namespace Payment.Worker.Workers;

public sealed class PaymentRequestedConsumerWorker : BackgroundService
{
    private readonly ILogger<PaymentRequestedConsumerWorker> _logger;
    private readonly RabbitMqPaymentRequestedConsumer _consumer;

    public PaymentRequestedConsumerWorker(
        ILogger<PaymentRequestedConsumerWorker> logger,
        RabbitMqPaymentRequestedConsumer consumer)
    {
        _logger = logger;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Payment consumer worker started");
        
        await _consumer.StartAsync(stoppingToken);
    }
}