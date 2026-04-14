# Order Saga

## Type
- Orchestration-based Saga
- Orchestrator: Order Service

---

## Saga State Machine

States:
- Created
- InventoryReserved
- PaymentCompleted
- ShippingArranged
- Completed
- Failed

---

## Steps

1. Reserve Inventory
2. Process Payment
3. Arrange Shipping

---

## Compensation

- Release Inventory
- Refund Payment
- Cancel Shipment

---

## Reliability Guarantees

- Saga state is persisted
- Duplicate events are ignored
- Out-of-order events are handled safely
- Saga survives service restarts