using System.Text;
using System.Text.Json;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shipping.Application.Shipments;
using Shipping.Infrastructure.Options;

namespace Shipping.Infrastructure.Messaging;

public class RabbitMqShipmentRequestedConsumer
{
    private readonly ShippingRabbitMqOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RabbitMqShipmentRequestedConsumer> _logger;

    public RabbitMqShipmentRequestedConsumer(
        IOptions<ShippingRabbitMqOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMqShipmentRequestedConsumer> logger)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            Port = _options.Port
        };

        var connection = await factory.CreateConnectionAsync(cancellationToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: _options.ShipmentRequestedQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: _options.ShipmentRequestedQueueName,
            exchange: _options.ExchangeName,
            routingKey: _options.ShipmentRequestedRoutingKey,
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, args) =>
        {
            try
            {
                var body = Encoding.UTF8.GetString(args.Body.ToArray());

                var envelope = JsonSerializer.Deserialize<EventEnvelope<ShipmentRequestedIntegrationEvent>>(
                    body,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });

                if (envelope is null)
                {
                    _logger.LogError("Failed to deserialize ShipmentRequested event.");

                    await channel.BasicNackAsync(
                        args.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken);

                    return;
                }

                using var scope = _scopeFactory.CreateScope();

                var handler = scope.ServiceProvider
                    .GetRequiredService<ShipmentRequestedHandler>();

                await handler.HandleAsync(envelope, cancellationToken);

                await channel.BasicAckAsync(
                    args.DeliveryTag,
                    multiple: false,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ShipmentRequested event.");

                await channel.BasicNackAsync(
                    args.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue: _options.ShipmentRequestedQueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Shipment consumer started. Queue: {QueueName}, RoutingKey: {RoutingKey}",
            _options.ShipmentRequestedQueueName,
            _options.ShipmentRequestedRoutingKey);
    }
}