using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BuildingBlocks.Messaging.RabbitMQ;

public abstract class RabbitMqConsumerBase<TEvent>
    where TEvent : IIntegrationEvent
{
    private readonly RabbitMqConsumerOptions _consumerOptions;
    private readonly ILogger _logger;

    protected RabbitMqConsumerBase(
        IOptions<RabbitMqConsumerOptions> consumerOptions,
        ILogger logger)
    {
        _consumerOptions = consumerOptions.Value;
        _logger = logger;
    }

    protected abstract string QueueName { get; }
    protected abstract string ExchangeName { get; }
    protected abstract string RoutingKey { get; }

    protected abstract ConnectionFactory CreateConnectionFactory();

    protected abstract Task HandleAsync(
        EventEnvelope<TEvent> envelope,
        CancellationToken cancellationToken);

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var factory = CreateConnectionFactory();

        var connection = await factory.CreateConnectionAsync(cancellationToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var deadLetterQueueName = $"{QueueName}.dlq";

        await channel.QueueDeclareAsync(
            queue: deadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = "",
                ["x-dead-letter-routing-key"] = deadLetterQueueName
            },
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: RoutingKey,
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, args) =>
        {
            try
            {
                var body = Encoding.UTF8.GetString(args.Body.ToArray());

                var envelope = JsonSerializer.Deserialize<EventEnvelope<TEvent>>(
                    body,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (envelope is null)
                {
                    _logger.LogError(
                        "Failed to deserialize message. Queue: {QueueName}, RoutingKey: {RoutingKey}",
                        QueueName,
                        RoutingKey);

                    await channel.BasicRejectAsync(
                        args.DeliveryTag,
                        requeue: false,
                        cancellationToken);

                    return;
                }

                for (var retryAttempt = 1; retryAttempt <= _consumerOptions.MaxRetryCount; retryAttempt++)
                {
                    try
                    {
                        await HandleAsync(envelope, cancellationToken);

                        await channel.BasicAckAsync(
                            args.DeliveryTag,
                            multiple: false,
                            cancellationToken);

                        return;
                    }

                    catch (Exception exception)
                    {
                        _logger.LogWarning(
                            exception,
                            "Failed to process message." +
                            " EventType: {EventType}, Queue: {QueueName}," +
                            " Attempt: {RetryAttempt}/{MaxRetryCount}," +
                            " MessageId: {MessageId}," +
                            " CorrelationId: {CorrelationId}",
                            envelope.EventType,
                            QueueName,
                            retryAttempt,
                            _consumerOptions.MaxRetryCount,
                            envelope.MessageId,
                            envelope.CorrelationId);

                        if (retryAttempt == _consumerOptions.MaxRetryCount)
                            break;

                        await Task.Delay(
                            _consumerOptions.RetryDelayMilliseconds,
                            cancellationToken);
                    }
                }

                _logger.LogError(
                    "Message moved to DLQ after exhausting retries. EventType: {EventType}, Queue: {QueueName}, DLQ: {DeadLetterQueueName}, MessageId: {MessageId}, CorrelationId: {CorrelationId}",
                    envelope.EventType,
                    QueueName,
                    deadLetterQueueName,
                    envelope.MessageId,
                    envelope.CorrelationId);

                await channel.BasicRejectAsync(
                    args.DeliveryTag,
                    requeue: false,
                    cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Unexpected error while consuming message. Queue: {QueueName}, RoutingKey: {RoutingKey}",
                    QueueName,
                    RoutingKey);

                await channel.BasicRejectAsync(
                    args.DeliveryTag,
                    requeue: false,
                    cancellationToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "RabbitMQ consumer started. EventType: {EventType}, Queue: {QueueName}, RoutingKey: {RoutingKey}, DLQ: {DeadLetterQueueName}",
            typeof(TEvent).Name,
            QueueName,
            RoutingKey,
            deadLetterQueueName);
    }
}