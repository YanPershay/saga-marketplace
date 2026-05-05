using Notification.Infrastructure.Messaging;

namespace Notification.Worker.Workers;

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
        _logger.LogInformation("RabbitMq OrderCreatedConsumer worker started");
        
        await _consumer.StartAsync(stoppingToken);
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}