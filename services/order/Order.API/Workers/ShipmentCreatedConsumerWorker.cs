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

        await _consumer.StartAsync(stoppingToken);
    }
}