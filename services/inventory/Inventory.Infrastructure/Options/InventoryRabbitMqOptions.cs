namespace Inventory.Infrastructure.Options;

public sealed class InventoryRabbitMqOptions
{
    public string HostName { get; set; } = null!;
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string VirtualHost { get; set; } = "/";

    public string ExchangeName { get; set; } = "marketplace.domain.events";

    public string OrderCreatedQueueName { get; set; } = "inventory.order-created";
    public string OrderCreatedRoutingKey { get; set; } = "order.created";
}