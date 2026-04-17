using Catalog.Application.Abstractions;
using Catalog.Application.Products.CreateProduct;
using Catalog.Application.Products.DeleteProduct;
using Catalog.Application.Products.GetProductById;
using Catalog.Application.Products.GetProducts;
using Catalog.Application.Products.UpdateProduct;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CatalogDb")));

services.AddScoped<IProductRepository, ProductRepository>();
services.AddScoped<CreateProductHandler>();
services.AddScoped<GetProductsHandler>();
services.AddScoped<GetProductByIdHandler>();
services.AddScoped<UpdateProductHandler>();
services.AddScoped<DeleteProductHandler>();

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

app.Run();