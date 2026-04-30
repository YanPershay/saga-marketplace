using Catalog.Application.Abstractions;
using Catalog.Application.Abstractions.AI;
using Catalog.Application.Products.CreateProduct;
using Catalog.Application.Products.DeleteProduct;
using Catalog.Application.Products.GetProductById;
using Catalog.Application.Products.GetProductRecommendations;
using Catalog.Application.Products.GetProducts;
using Catalog.Application.Products.UpdateProduct;
using Catalog.Infrastructure.Clients.AI;
using Catalog.Infrastructure.HealthChecks;
using Catalog.Infrastructure.Options;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddCheck<AiServiceHealthCheck>("ai-service");

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CatalogDb")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<GetProductsHandler>();
builder.Services.AddScoped<GetProductByIdHandler>();
builder.Services.AddScoped<UpdateProductHandler>();
builder.Services.AddScoped<DeleteProductHandler>();

builder.Services.AddScoped<GetProductRecommendationsHandler>();

builder.Services.AddSingleton<IValidateOptions<AiServiceOptions>, AiServiceOptionsValidator>();

builder.Services.AddOptions<AiServiceOptions>()
    .Bind(builder.Configuration.GetSection("AiService"))
    .ValidateOnStart();

builder.Services.AddHttpClient<IAiRecommendationClient, AiRecommendationHttpClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<AiServiceOptions>>().Value;

    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
});

builder.Services.AddHttpClient<AiServiceHealthCheck>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<AiServiceOptions>>().Value;

    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(2);
});

builder.Services.AddOptions<ProductRecommendationsOptions>()
    .Bind(builder.Configuration.GetSection("ProductRecommendations"))
    .Validate(options => options.MaxCandidates > 0, "MaxCandidates must be greater than 0.")
    .Validate(options => options.FallbackCount > 0, "FallbackCount must be greater than 0.")
    .Validate(options => options.FallbackCount <= options.MaxCandidates, "FallbackCount cannot be greater than MaxCandidates.")
    .ValidateOnStart();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    dbContext.Database.Migrate();
}

app.MapControllers();

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

app.Run();