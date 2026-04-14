# Events

## Event Envelope

Each event contains:

- MessageId
- CorrelationId
- CausationId
- EventType
- Version
- OccurredAt
- Payload

---

## Ownership

- Events belong to the emitting service
- Only the owner can change the contract
- Consumers adapt to changes

---

## Core Events (Draft)

- OrderCreated
- InventoryReserved
- InventoryReservationFailed
- PaymentSucceeded
- PaymentFailed
- ShippingArranged
- ShippingFailed
- OrderCompleted
- OrderCancelled