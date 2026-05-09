namespace Order.Infrastructure.Options;

public sealed class OrderRabbitMqOptions
{
    public string HostName { get; set; } = null!;
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string VirtualHost { get; set; } = "/";

    public string ExchangeName { get; set; } = "marketplace.domain.events";

    public string InventoryReservedQueueName { get; set; } = "order.inventory-reserved";
    public string InventoryReservedRoutingKey { get; set; } = "inventory.reserved";

    public string InventoryReservationFailedQueueName { get; set; } = "order.inventory-reservation-failed";
    public string InventoryReservationFailedRoutingKey { get; set; } = "inventory.reservation.failed";

    public string PaymentSucceededQueueName { get; set; } = "order.payment-succeeded";
    public string PaymentSucceededRoutingKey { get; set; } = "payment.succeeded";

    public string PaymentFailedQueueName { get; set; } = "order.payment-failed";
    public string PaymentFailedRoutingKey { get; set; } = "payment.failed";
}