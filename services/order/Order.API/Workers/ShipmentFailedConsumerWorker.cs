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

        await _consumer.StartAsync(stoppingToken);
    }
}