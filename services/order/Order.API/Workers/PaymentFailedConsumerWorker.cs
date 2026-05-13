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

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _consumer.StartAsync(stoppingToken);

                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("PaymentFailed consumer worker stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "PaymentFailed consumer worker failed. Retrying in 5 seconds.");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}