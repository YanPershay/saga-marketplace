using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Messaging.RabbitMQ;
using Inventory.Application.Orders;
using Inventory.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Inventory.Infrastructure.Messaging;

public sealed class RabbitMqOrderCreatedConsumer
    : RabbitMqConsumerBase<OrderCreatedIntegrationEvent>
{
    private readonly InventoryRabbitMqOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitMqOrderCreatedConsumer(
        IOptions<InventoryRabbitMqOptions> options,
        IOptions<RabbitMqConsumerOptions> consumerOptions,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMqOrderCreatedConsumer> logger)
        : base(consumerOptions, logger)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
    }

    protected override string QueueName => _options.OrderCreatedQueueName;
    protected override string RoutingKey => _options.OrderCreatedRoutingKey;
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
        EventEnvelope<OrderCreatedIntegrationEvent> envelope,
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var handler = scope.ServiceProvider
            .GetRequiredService<OrderCreatedHandler>();

        await handler.HandleAsync(envelope, cancellationToken);
    }
}