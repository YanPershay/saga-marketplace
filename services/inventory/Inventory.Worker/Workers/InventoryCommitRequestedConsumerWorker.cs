using Inventory.Infrastructure.Messaging;

namespace Inventory.Worker.Workers;

public sealed class InventoryCommitRequestedConsumerWorker : BackgroundService
{
    private readonly RabbitMqInventoryCommitRequestedConsumer _consumer;
    private readonly ILogger<InventoryCommitRequestedConsumerWorker> _logger;

    public InventoryCommitRequestedConsumerWorker(
        RabbitMqInventoryCommitRequestedConsumer consumer,
        ILogger<InventoryCommitRequestedConsumerWorker> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("InventoryCommitRequested consumer worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _consumer.StartAsync(stoppingToken);

                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("InventoryCommitRequested consumer worker stopped.");
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    exception,
                    "InventoryCommitRequested consumer failed. Retrying in 5 seconds.");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}