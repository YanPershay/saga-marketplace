using Order.Infrastructure.Messaging;

namespace Order.API.Workers;

public sealed class InventoryReservedConsumerWorker : BackgroundService
{
    private readonly RabbitMqInventoryReservedConsumer _consumer;
    private readonly ILogger<InventoryReservedConsumerWorker> _logger;

    public InventoryReservedConsumerWorker(
        RabbitMqInventoryReservedConsumer consumer,
        ILogger<InventoryReservedConsumerWorker> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("InventoryReserved consumer worker started.");

        await _consumer.StartAsync(stoppingToken);
    }
}