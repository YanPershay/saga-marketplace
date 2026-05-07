using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;

namespace Inventory.Application.Abstractions;

public interface IInventoryUnitOfWork
{
    Task<bool> HasInboxMessageAsync(
        Guid messageId,
        string consumerName,
        CancellationToken cancellationToken = default);

    Task ReserveInventoryAndSaveOutboxAsync(
        EventEnvelope<OrderCreatedIntegrationEvent> envelope,
        string consumerName,
        CancellationToken cancellationToken = default);
}