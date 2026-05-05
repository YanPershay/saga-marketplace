using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BuildingBlocks.Messaging.RabbitMQ;

public sealed class RabbitMqEventPublisher : IEventPublisher
{
    private readonly RabbitMqOptions _options;
    
    public RabbitMqEventPublisher(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }

    public async Task PublishAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default
    ) where TEvent : IIntegrationEvent
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            Port = _options.Port
        };
        
        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        
        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);
        
        var envelope = EventEnvelope<TEvent>.Create(@event);

        var json = JsonSerializer.Serialize(envelope, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        var body = Encoding.UTF8.GetBytes(json);

        var routingKey = GetRoutingKey<TEvent>();

        await channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: routingKey,
            body: body,
            cancellationToken
        );
    }

    private static string GetRoutingKey<TEvent>()
        where TEvent : IIntegrationEvent
    {
        return typeof(TEvent).Name switch
        {
            nameof(Events.OrderCreatedIntegrationEvent) => "order.created",
            _ => throw new InvalidOperationException(
                $"Routing key is not configured for event type {typeof(TEvent).Name}.")
        };
    }
}