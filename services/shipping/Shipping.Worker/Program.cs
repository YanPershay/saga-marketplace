using Microsoft.EntityFrameworkCore;
using Shipping.Application.Abstractions;
using Shipping.Application.Shipments;
using Shipping.Infrastructure.Messaging;
using Shipping.Infrastructure.Persistence;
using Shipping.Worker.Workers;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.RabbitMQ;
using Shipping.Application.Options;
using Shipping.Infrastructure.Options;
using Shipping.Infrastructure.Persistence.Outbox;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<ShippingDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("ShippingDatabase"));
});

builder.Services.Configure<ShippingRabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMq"));

builder.Services.Configure<ShippingSimulationOptions>(
    builder.Configuration.GetSection("ShippingSimulation"));

builder.Services.AddOptions<RabbitMqOptions>()
    .Bind(builder.Configuration.GetSection("RabbitMq"))
    .ValidateOnStart();

builder.Services.AddScoped<IShippingUnitOfWork, ShippingUnitOfWork>();

builder.Services.AddScoped<ShipmentRequestedHandler>();

builder.Services.AddScoped<IRawMessagePublisher, RabbitMqRawMessagePublisher>();

builder.Services.AddScoped<OutboxPublisher>();

builder.Services.AddSingleton<RabbitMqShipmentRequestedConsumer>();

builder.Services.AddHostedService<ShipmentRequestedConsumerWorker>();

builder.Services.AddHostedService<OutboxPublisherWorker>();

var host = builder.Build();
host.Run();