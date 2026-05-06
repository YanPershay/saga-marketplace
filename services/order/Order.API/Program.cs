using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using Order.API.Workers;
using Order.Application.Abstractions;
using Order.Application.Orders.CreateOrder;
using Order.Application.Orders.GetOrderById;
using Order.Infrastructure.Persistence;
using Order.Infrastructure.Persistence.Outbox;
using Order.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderDb")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<CreateOrderHandler>();
builder.Services.AddScoped<GetOrderByIdHandler>();

builder.Services.AddScoped<IOrderUnitOfWork, OrderUnitOfWork>();
builder.Services.AddScoped<IRawMessagePublisher, RabbitMqRawMessagePublisher>();

builder.Services.AddScoped<OutboxPublisher>();

builder.Services.AddHostedService<OutboxPublisherWorker>();

builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();