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

        await _consumer.StartAsync(stoppingToken);
    }
}