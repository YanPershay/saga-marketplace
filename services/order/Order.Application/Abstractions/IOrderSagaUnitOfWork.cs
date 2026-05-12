using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;

namespace Order.Application.Abstractions;

public interface IOrderSagaUnitOfWork
{
    Task<bool> HasInboxMessageAsync(
        Guid messageId,
        string consumerName,
        CancellationToken cancellationToken = default);

    Task HandleInventoryReservedAsync(
        EventEnvelope<InventoryReservedIntegrationEvent> envelope,
        string consumerName,
        CancellationToken cancellationToken = default);

    Task HandleInventoryReservationFailedAsync(
        EventEnvelope<InventoryReservationFailedIntegrationEvent> envelope,
        string consumerName,
        CancellationToken cancellationToken = default);

    Task HandlePaymentSucceededAsync(
        EventEnvelope<PaymentSucceededIntegrationEvent> envelope,
        string consumerName,
        CancellationToken cancellationToken = default);

    Task HandlePaymentFailedAsync(
        EventEnvelope<PaymentFailedIntegrationEvent> envelope,
        string consumerName,
        CancellationToken cancellationToken = default);
    
    Task HandleShipmentCreatedAsync(
        EventEnvelope<ShipmentCreatedIntegrationEvent> envelope,
        string consumerName,
        CancellationToken cancellationToken = default);

    Task HandleShipmentFailedAsync(
        EventEnvelope<ShipmentFailedIntegrationEvent> envelope,
        string consumerName,
        CancellationToken cancellationToken = default);
}