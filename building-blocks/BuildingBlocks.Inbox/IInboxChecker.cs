namespace BuildingBlocks.Inbox;

public interface IInboxChecker
{
    Task<bool> IsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);
}