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

public sealed class RabbitMqInventoryReservedConsumer
    : RabbitMqConsumerBase<InventoryReservedIntegrationEvent>
{
    private readonly OrderRabbitMqOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitMqInventoryReservedConsumer(
        IOptions<OrderRabbitMqOptions> options,
        IOptions<RabbitMqConsumerOptions> consumerOptions,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMqInventoryReservedConsumer> logger)
        : base(consumerOptions, logger)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
    }

    protected override string QueueName => _options.InventoryReservedQueueName;

    protected override string RoutingKey => _options.InventoryReservedRoutingKey;

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
        EventEnvelope<InventoryReservedIntegrationEvent> envelope,
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var handler = scope.ServiceProvider
            .GetRequiredService<InventoryReservedHandler>();

        await handler.HandleAsync(envelope, cancellationToken);
    }
}