namespace Order.Domain.Orders;

public enum OrderStatus
{
    Pending = 0,
    AwaitingInventory = 1,
    InventoryReserved = 2,
    AwaitingPayment = 3,
    PaymentSucceeded = 4,
    PaymentFailed = 5,
    AwaitingShipment = 6,
    ShipmentCreated = 7,
    ShipmentFailed = 8,
    Cancelled = 9,
    Completed = 10
}