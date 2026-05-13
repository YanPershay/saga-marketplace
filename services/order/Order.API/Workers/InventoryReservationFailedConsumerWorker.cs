using Order.Infrastructure.Messaging;

namespace Order.API.Workers;

public sealed class InventoryReservationFailedConsumerWorker : BackgroundService
{
    private readonly RabbitMqInventoryReservationFailedConsumer _consumer;
    private readonly ILogger<InventoryReservationFailedConsumerWorker> _logger;

    public InventoryReservationFailedConsumerWorker(
        RabbitMqInventoryReservationFailedConsumer consumer,
        ILogger<InventoryReservationFailedConsumerWorker> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("InventoryReservationFailed consumer worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _consumer.StartAsync(stoppingToken);

                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("InventoryReservationFailed consumer worker stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "InventoryReservationFailed consumer failed. Retrying in 5 seconds.");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}