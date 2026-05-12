using Shipping.Infrastructure.Messaging;

namespace Shipping.Worker.Workers;

public sealed class ShipmentRequestedConsumerWorker : BackgroundService
{
    private readonly ILogger<ShipmentRequestedConsumerWorker> _logger;
    private readonly RabbitMqShipmentRequestedConsumer _consumer;
    
    public ShipmentRequestedConsumerWorker(
        ILogger<ShipmentRequestedConsumerWorker> logger,
        RabbitMqShipmentRequestedConsumer consumer)
    {
        _logger = logger;
        _consumer = consumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        
        return _consumer.StartAsync(stoppingToken);
    }
}