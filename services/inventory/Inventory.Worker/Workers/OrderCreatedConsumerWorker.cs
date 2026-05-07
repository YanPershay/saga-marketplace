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
        
        await  _consumer.StartAsync(stoppingToken);
    }
}