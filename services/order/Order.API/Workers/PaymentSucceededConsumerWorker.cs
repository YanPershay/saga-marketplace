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

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _consumer.StartAsync(stoppingToken);

                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("PaymentSucceeded consumer worker stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "PaymentSucceeded consumer failed. Retrying in 5 seconds.");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}