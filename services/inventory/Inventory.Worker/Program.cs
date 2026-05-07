using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.RabbitMQ;
using Inventory.Application.Abstractions;
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

builder.Services.AddOptions<RabbitMqOptions>()
    .Bind(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddScoped<OrderCreatedHandler>();

builder.Services.AddSingleton<RabbitMqOrderCreatedConsumer>();

builder.Services.AddHostedService<OrderCreatedConsumerWorker>();

builder.Services.AddScoped<OutboxPublisher>();
builder.Services.AddHostedService<OutboxPublisherWorker>();

builder.Services.AddScoped<IRawMessagePublisher, RabbitMqRawMessagePublisher>();

var host = builder.Build();
host.Run();