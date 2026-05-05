using Notification.Application.Orders;
using Notification.Infrastructure.Messaging;
using Notification.Infrastructure.Options;
using Notification.Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<NotificationRabbitMqOptions>()
    .Bind(builder.Configuration.GetSection("RabbitMq"))
    .Validate(options => !string.IsNullOrWhiteSpace(options.HostName), "RabbitMQ HostName is required.")
    .Validate(options => options.Port > 0, "RabbitMQ Port must be greater than 0.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.UserName), "RabbitMQ UserName is required.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.Password), "RabbitMQ Password is required.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.VirtualHost), "RabbitMQ VirtualHost is required.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.ExchangeName), "RabbitMQ ExchangeName is required.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.QueueName), "RabbitMQ QueueName is required.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.RoutingKey), "RabbitMQ RoutingKey is required.")
    .ValidateOnStart();

builder.Services.AddScoped<OrderCreatedHandler>();
builder.Services.AddSingleton<RabbitMqOrderCreatedConsumer>();
builder.Services.AddHostedService<OrderCreatedConsumerWorker>();

var host = builder.Build();
host.Run();