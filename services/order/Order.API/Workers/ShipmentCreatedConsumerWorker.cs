using Order.Infrastructure.Messaging;

namespace Order.API.Workers;

public sealed class ShipmentCreatedConsumerWorker : BackgroundService
{
    private readonly RabbitMqShipmentCreatedConsumer _consumer;
    private readonly ILogger<ShipmentCreatedConsumerWorker> _logger;

    public ShipmentCreatedConsumerWorker(
        RabbitMqShipmentCreatedConsumer consumer,
        ILogger<ShipmentCreatedConsumerWorker> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ShipmentCreated consumer worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _consumer.StartAsync(stoppingToken);

                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("ShipmentCreated consumer worker stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "ShipmentCreated consumer worker failed. Retrying in 5 seconds.");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}