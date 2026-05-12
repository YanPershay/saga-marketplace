using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;

namespace Shipping.Application.Abstractions;

public interface IShippingUnitOfWork
{
    Task<bool> HasInboxMessageAsync(
        Guid messageId,
        string consumerName,
        CancellationToken cancellationToken = default);

    Task CreateShipmentAndSaveOutboxAsync(
        EventEnvelope<ShipmentRequestedIntegrationEvent> envelope,
        string consumerName,
        bool forceFailure,
        CancellationToken cancellationToken = default);
}