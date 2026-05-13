namespace BuildingBlocks.Messaging.RabbitMQ;

public sealed class RabbitMqConsumerOptions
{
    public int MaxRetryCount { get; set; } = 3;

    public int RetryDelayMilliseconds { get; set; } = 1000;

    public bool EnableDeadLetterQueue { get; set; } = true;
}