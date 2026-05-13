using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Messaging.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shipping.Application.Shipments;
using Shipping.Infrastructure.Options;

namespace Shipping.Infrastructure.Messaging;

public sealed class RabbitMqShipmentRequestedConsumer
    : RabbitMqConsumerBase<ShipmentRequestedIntegrationEvent>
{
    private readonly ShippingRabbitMqOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitMqShipmentRequestedConsumer(
        IOptions<ShippingRabbitMqOptions> options,
        IOptions<RabbitMqConsumerOptions> consumerOptions,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMqShipmentRequestedConsumer> logger)
        : base(consumerOptions, logger)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
    }

    protected override string QueueName => _options.ShipmentRequestedQueueName;
    protected override string RoutingKey => _options.ShipmentRequestedRoutingKey;
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
        EventEnvelope<ShipmentRequestedIntegrationEvent> envelope,
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var handler = scope.ServiceProvider
            .GetRequiredService<ShipmentRequestedHandler>();

        await handler.HandleAsync(envelope, cancellationToken);
    }
}