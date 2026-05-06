namespace BuildingBlocks.Messaging;

public interface IRawMessagePublisher
{
    Task PublishAsync(
        string routingKey, string payload, CancellationToken ct = default);
}