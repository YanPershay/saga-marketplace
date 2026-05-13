using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Messaging.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Order.Application.Saga;
using Order.Infrastructure.Options;
using RabbitMQ.Client;

namespace Order.Infrastructure.Messaging;

public sealed class RabbitMqShipmentFailedConsumer
    : RabbitMqConsumerBase<ShipmentFailedIntegrationEvent>
{
    private readonly OrderRabbitMqOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitMqShipmentFailedConsumer(
        IOptions<OrderRabbitMqOptions> options,
        IOptions<RabbitMqConsumerOptions> consumerOptions,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMqShipmentFailedConsumer> logger)
        : base(consumerOptions, logger)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
    }

    protected override string QueueName => _options.ShipmentFailedQueueName;

    protected override string RoutingKey => _options.ShipmentFailedRoutingKey;

    protected override string ExchangeName => _options.ExchangeName;

    protected override ConnectionFactory CreateConnectionFactory()
    {
        return new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };
    }

    protected override async Task HandleAsync(
        EventEnvelope<ShipmentFailedIntegrationEvent> envelope,
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var handler = scope.ServiceProvider
            .GetRequiredService<ShipmentFailedHandler>();

        await handler.HandleAsync(envelope, cancellationToken);
    }
}