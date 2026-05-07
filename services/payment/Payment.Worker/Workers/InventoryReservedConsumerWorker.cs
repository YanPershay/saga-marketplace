using Payment.Infrastructure.Messaging;

namespace Payment.Worker.Workers;

public sealed class InventoryReservedConsumerWorker : BackgroundService
{
    private readonly ILogger<InventoryReservedConsumerWorker> _logger;
    private readonly RabbitMqInventoryReservedConsumer _consumer;

    public InventoryReservedConsumerWorker(
        ILogger<InventoryReservedConsumerWorker> logger,
        RabbitMqInventoryReservedConsumer consumer)
    {
        _logger = logger;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Payment consumer worker started");
        
        await _consumer.StartAsync(stoppingToken);
    }
}