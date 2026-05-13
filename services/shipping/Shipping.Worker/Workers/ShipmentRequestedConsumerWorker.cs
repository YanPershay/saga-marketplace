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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ShipmentRequested consumer worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _consumer.StartAsync(stoppingToken);

                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("ShipmentCreated consumer worker stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "ShipmentCreated consumer failed. Retrying in 5 seconds.");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}