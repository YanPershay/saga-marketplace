using Order.Infrastructure.Messaging;

namespace Order.API.Workers;

public sealed class PaymentSucceededConsumerWorker : BackgroundService
{
    private readonly RabbitMqPaymentSucceededConsumer _consumer;
    private readonly ILogger<PaymentSucceededConsumerWorker> _logger;

    public PaymentSucceededConsumerWorker(
        RabbitMqPaymentSucceededConsumer consumer,
        ILogger<PaymentSucceededConsumerWorker> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PaymentSucceeded consumer worker started.");

        await _consumer.StartAsync(stoppingToken);
    }
}