using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.RabbitMQ;
using BuildingBlocks.Observability;
using Inventory.Application.Abstractions;
using Inventory.Application.Inventory;
using Inventory.Application.Orders;
using Inventory.Infrastructure.Messaging;
using Inventory.Infrastructure.Options;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Persistence.Outbox;
using Inventory.Worker.Workers;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<InventoryDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("InventoryDatabase"));
});

builder.Services.AddScoped<IInventoryUnitOfWork, InventoryUnitOfWork>();

builder.Services.Configure<InventoryRabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddOptions<RabbitMqConsumerOptions>()
    .Bind(builder.Configuration.GetSection("RabbitMqConsumer"))
    .Validate(options => options.MaxRetryCount > 0, "MaxRetryCount must be greater than 0.")
    .Validate(options => options.RetryDelayMilliseconds >= 0, "RetryDelayMilliseconds cannot be negative.")
    .ValidateOnStart();

builder.Services.AddOptions<RabbitMqOptions>()
    .Bind(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddScoped<OrderCreatedHandler>();

builder.Services.AddSingleton<RabbitMqOrderCreatedConsumer>();

builder.Services.AddHostedService<OrderCreatedConsumerWorker>();

builder.Services.AddScoped<OutboxPublisher>();
builder.Services.AddHostedService<OutboxPublisherWorker>();

builder.Services.AddScoped<IRawMessagePublisher, RabbitMqRawMessagePublisher>();

builder.Services.AddScoped<InventoryCommitRequestedHandler>();

builder.Services.AddSingleton<RabbitMqInventoryCommitRequestedConsumer>();

builder.Services.AddHostedService<InventoryCommitRequestedConsumerWorker>();

builder.Services.AddOpenTelemetryObservability("Inventory.Worker");

var host = builder.Build();
host.Run();