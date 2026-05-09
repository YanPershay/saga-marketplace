using Order.Infrastructure.Messaging;

namespace Order.API.Workers;

public sealed class PaymentFailedConsumerWorker : BackgroundService
{
    private readonly RabbitMqPaymentFailedConsumer _consumer;
    private readonly ILogger<PaymentFailedConsumerWorker> _logger;

    public PaymentFailedConsumerWorker(
        RabbitMqPaymentFailedConsumer consumer,
        ILogger<PaymentFailedConsumerWorker> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PaymentFailed consumer worker started.");

        await _consumer.StartAsync(stoppingToken);
    }
}