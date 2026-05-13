using Inventory.Infrastructure.Messaging;

namespace Inventory.Worker.Workers;

public sealed class OrderCreatedConsumerWorker : BackgroundService
{
    private readonly RabbitMqOrderCreatedConsumer _consumer;
    private readonly ILogger<OrderCreatedConsumerWorker> _logger;

    public OrderCreatedConsumerWorker(
        RabbitMqOrderCreatedConsumer consumer,
        ILogger<OrderCreatedConsumerWorker> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Inventory OrderCreated consumer worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _consumer.StartAsync(stoppingToken);

                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Inventory OrderCreated consumer worker stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Inventory OrderCreated consumer failed. Retrying in 5 seconds.");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}