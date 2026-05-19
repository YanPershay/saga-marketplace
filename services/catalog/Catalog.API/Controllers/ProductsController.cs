using Catalog.API.Contracts.Requests;
using Catalog.API.Contracts.Responses;
using Catalog.API.Mappings;
using Catalog.API.Messaging;
using Catalog.Application.Products.CreateProduct;
using Catalog.Application.Products.DeleteProduct;
using Catalog.Application.Products.GetProductById;
using Catalog.Application.Products.GetProductRecommendations;
using Catalog.Application.Products.GetProducts;
using Catalog.Application.Products.UpdateProduct;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("products")]
public class ProductsController(
    CreateProductHandler createHandler,
    GetProductsHandler getProductsHandler,
    GetProductByIdHandler getByIdHandler,
    UpdateProductHandler updateHandler,
    DeleteProductHandler deleteHandler,
    GetProductRecommendationsHandler getProductRecommendationsHandler
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ProductResponse>>> GetProducts()
    {
        var query = new GetProductsQuery();
        var products = await getProductsHandler.HandleAsync(query);
        var response = products.Select(p => p.ToResponse()).ToArray();
        
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetProductById(Guid id)
    {
        var query = new GetProductByIdQuery(id);
        var product = await getByIdHandler.HandleAsync(query);
        
        if (product is null)
            return NotFound();
        
        return Ok(product.ToResponse());
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var command = request.ToCommand();
        var result = await createHandler.HandleAsync(command);
        return CreatedAtAction(nameof(GetProductById), new { id = result }, new { id = result });
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        var command = request.ToCommand(id);

        await updateHandler.HandleAsync(command);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var command = new DeleteProductCommand(id);
        await deleteHandler.HandleAsync(command);
        
        return NoContent();
    }
}