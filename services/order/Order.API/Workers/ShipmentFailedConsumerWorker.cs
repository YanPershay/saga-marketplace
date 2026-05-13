using Order.Infrastructure.Messaging;

namespace Order.API.Workers;

public sealed class ShipmentFailedConsumerWorker : BackgroundService
{
    private readonly RabbitMqShipmentFailedConsumer _consumer;
    private readonly ILogger<ShipmentFailedConsumerWorker> _logger;

    public ShipmentFailedConsumerWorker(
        RabbitMqShipmentFailedConsumer consumer,
        ILogger<ShipmentFailedConsumerWorker> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ShipmentFailed consumer worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _consumer.StartAsync(stoppingToken);

                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("ShipmentFailed consumer worker stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "ShipmentFailed consumer worker failed. Retrying in 5 seconds.");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}