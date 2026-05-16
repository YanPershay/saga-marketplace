using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Messaging.RabbitMQ;
using Inventory.Application.Inventory;
using Inventory.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Inventory.Infrastructure.Messaging;

public sealed class RabbitMqInventoryCommitRequestedConsumer
    : RabbitMqConsumerBase<InventoryCommitRequestedIntegrationEvent>
{
    private readonly InventoryRabbitMqOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitMqInventoryCommitRequestedConsumer(
        IOptions<InventoryRabbitMqOptions> options,
        IOptions<RabbitMqConsumerOptions> consumerOptions,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMqInventoryCommitRequestedConsumer> logger)
        : base(consumerOptions, logger)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
    }

    protected override string QueueName => _options.InventoryCommitRequestedQueueName;

    protected override string RoutingKey => _options.InventoryCommitRequestedRoutingKey;

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
        EventEnvelope<InventoryCommitRequestedIntegrationEvent> envelope,
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var handler = scope.ServiceProvider
            .GetRequiredService<InventoryCommitRequestedHandler>();

        await handler.HandleAsync(envelope, cancellationToken);
    }
}