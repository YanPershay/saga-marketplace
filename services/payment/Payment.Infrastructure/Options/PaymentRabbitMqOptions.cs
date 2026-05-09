namespace Payment.Infrastructure.Options;

public sealed class PaymentRabbitMqOptions
{
    public string HostName { get; set; } = null!;
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string VirtualHost { get; set; } = "/";

    public string ExchangeName { get; set; } = "marketplace.domain.events";

    public string PaymentRequestedQueueName { get; set; } = "payment.payment-requested";
    public string PaymentRequestedRoutingKey { get; set; } = "payment.requested";
}