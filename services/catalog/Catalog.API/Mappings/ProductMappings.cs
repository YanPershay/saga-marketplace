using System;
using Catalog.API.Contracts.Requests;
using Catalog.API.Contracts.Responses;
using Catalog.Application.Products.CreateProduct;
using Catalog.Application.Products.GetProductRecommendations;
using Catalog.Application.Products.UpdateProduct;
using Catalog.Domain;

namespace Catalog.API.Mappings;

public static class ProductMappings
{
    public static CreateProductCommand ToCommand(this CreateProductRequest request)
    {
        return new CreateProductCommand(
            request.Name,
            request.Description,
            request.Price,
            request.QuantityAvailable,
            request.Category);
    }
    
    public static UpdateProductCommand ToCommand(this UpdateProductRequest request, Guid id)
    {
        return new UpdateProductCommand(
            id,
            request.Name,
            request.Description,
            request.Price,
            request.QuantityAvailable,
            request.Category);
    }

    public static ProductResponse ToResponse(this Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.QuantityAvailable,
            product.CreatedAt);
    }
    
    public static ProductRecommendationResponse ToResponse(this ProductRecommendationResult result)
    {
        return new ProductRecommendationResponse(
            result.Id,
            result.Name,
            result.Description,
            result.Price,
            result.QuantityAvailable,
            result.CreatedAt,
            result.Reason);
    }
}