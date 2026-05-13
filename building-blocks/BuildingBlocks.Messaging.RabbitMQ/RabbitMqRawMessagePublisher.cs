using System.Diagnostics;
using System.Text;
using System.Text.Json;
using BuildingBlocks.Observability;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;

namespace BuildingBlocks.Messaging.RabbitMQ;

public sealed class RabbitMqRawMessagePublisher : IRawMessagePublisher
{
    private readonly RabbitMqOptions _options;
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    
    public RabbitMqRawMessagePublisher(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }

    public async Task PublishAsync(
        string routingKey, string payload,
        CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password,
            Port = _options.Port,
            VirtualHost = _options.VirtualHost,
        };
        
        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        
        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);
        
        var body = Encoding.UTF8.GetBytes(payload);
        
        var eventType = "unknown";
        var messageId = "unknown";
        var correlationId = "unknown";
        
        var traceParent = string.Empty;
        var traceState = string.Empty;

        try
        {
            using var document = JsonDocument.Parse(payload);
            var root = document.RootElement;
            
            if (root.TryGetProperty("traceParent", out var traceParentElement))
                traceParent = traceParentElement.GetString() ?? string.Empty;

            if (root.TryGetProperty("traceState", out var traceStateElement))
                traceState = traceStateElement.GetString() ?? string.Empty;

            if (root.TryGetProperty("eventType", out var eventTypeElement))
                eventType = eventTypeElement.GetString() ?? "unknown";

            if (root.TryGetProperty("messageId", out var messageIdElement))
                messageId = messageIdElement.GetString() ?? "unknown";

            if (root.TryGetProperty("correlationId", out var correlationIdElement))
                correlationId = correlationIdElement.GetString() ?? "unknown";
        }
        catch (JsonException)
        {
            // tracing metadata extraction is best-effort
        }
        
        ActivityContext parentContext = default;

        if (!string.IsNullOrWhiteSpace(traceParent))
        {
            ActivityContext.TryParse(
                traceParent,
                traceState,
                out parentContext);
        }
        
        using var activity = MessagingTelemetry.ActivitySource.StartActivity(
            $"publish {eventType}",
            ActivityKind.Producer,
            parentContext);

        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination.name", _options.ExchangeName);
        activity?.SetTag("messaging.rabbitmq.routing_key", routingKey);
        activity?.SetTag("messaging.operation", "publish");
        activity?.SetTag("messaging.message_id", messageId);
        activity?.SetTag("saga.correlation_id", correlationId);
        activity?.SetTag("event.type", eventType);
        
        var properties = new BasicProperties
        {
            Persistent = true,
            Headers = new Dictionary<string, object?>()
        };

        Propagator.Inject(
            new PropagationContext(
                activity?.Context ?? default,
                Baggage.Current),
            properties.Headers,
            static (headers, key, value) =>
            {
                headers[key] = value;
            });

        await channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }
}