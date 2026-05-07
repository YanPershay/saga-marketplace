using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;

namespace Payment.Application.Abstractions;

public interface IPaymentUnitOfWork
{
    Task<bool> HasInboxMessageAsync(
        Guid messageId,
        string consumerName,
        CancellationToken cancellationToken = default);

    Task ProcessPaymentAndSaveOutboxAsync(
        EventEnvelope<InventoryReservedIntegrationEvent> envelope,
        string consumerName,
        bool forceFailure,
        CancellationToken cancellationToken = default);
}