using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using Payment.Application.Abstractions;
using Payment.Application.Options;
using Payment.Application.Payments;
using Payment.Infrastructure.Messaging;
using Payment.Infrastructure.Options;
using Payment.Infrastructure.Persistence;
using Payment.Infrastructure.Persistence.Outbox;
using Payment.Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<PaymentDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PaymentDatabase"));
});

builder.Services.Configure<PaymentRabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMq"));

builder.Services.Configure<PaymentSimulationOptions>(
    builder.Configuration.GetSection("PaymentSimulation"));

builder.Services.AddOptions<RabbitMqOptions>()
    .Bind(builder.Configuration.GetSection("RabbitMq"))
    .ValidateOnStart();

builder.Services.AddScoped<IPaymentUnitOfWork, PaymentUnitOfWork>();

builder.Services.AddScoped<InventoryReservedHandler>();

builder.Services.AddScoped<IRawMessagePublisher, RabbitMqRawMessagePublisher>();

builder.Services.AddScoped<OutboxPublisher>();

builder.Services.AddSingleton<RabbitMqInventoryReservedConsumer>();

builder.Services.AddHostedService<InventoryReservedConsumerWorker>();

builder.Services.AddHostedService<OutboxPublisherWorker>();

var host = builder.Build();
host.Run();