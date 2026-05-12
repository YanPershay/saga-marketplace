namespace Shipping.Infrastructure.Options;

public sealed class ShippingRabbitMqOptions
{
    public string HostName { get; set; } = null!;
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string VirtualHost { get; set; } = "/";

    public string ExchangeName { get; set; } = "marketplace.domain.events";

    public string ShipmentRequestedQueueName { get; set; } =
        "shipping.shipment-requested";

    public string ShipmentRequestedRoutingKey { get; set; } =
        "shipment.requested";
}