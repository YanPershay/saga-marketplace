namespace Notification.Infrastructure.Options;

public sealed class NotificationRabbitMqOptions
{
    public string HostName { get; set; } = null!;
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string VirtualHost { get; set; } = "/";
    
    public string ExchangeName { get; set; } = "marketplace.domain.events";
    public string QueueName { get; set; } = "notification.order-created";
    public string RoutingKey { get; set; } = "order.created";
}