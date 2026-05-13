using System.Text;
using System.Text.Json;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Messaging.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payment.Application.Payments;
using Payment.Infrastructure.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Payment.Infrastructure.Messaging;

public sealed class RabbitMqPaymentRequestedConsumer
    : RabbitMqConsumerBase<PaymentRequestedIntegrationEvent>
{
    private readonly PaymentRabbitMqOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitMqPaymentRequestedConsumer(
        IOptions<PaymentRabbitMqOptions> options,
        IOptions<RabbitMqConsumerOptions> consumerOptions,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMqPaymentRequestedConsumer> logger)
        : base(consumerOptions, logger)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
    }

    protected override string QueueName => _options.PaymentRequestedQueueName;
    protected override string RoutingKey => _options.PaymentRequestedRoutingKey;
    protected override string ExchangeName => _options.ExchangeName;

    protected override ConnectionFactory CreateConnectionFactory()
    {
        return new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password,
            Port = _options.Port,
            VirtualHost = _options.VirtualHost,
        };
    }

    protected override async Task HandleAsync(
        EventEnvelope<PaymentRequestedIntegrationEvent> envelope, 
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var handler = scope
            .ServiceProvider
            .GetRequiredService<PaymentRequestedHandler>();

        await handler.HandleAsync(envelope, cancellationToken);
    }
}