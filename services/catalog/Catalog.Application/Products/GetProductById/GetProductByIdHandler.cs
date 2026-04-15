using Catalog.Application.Abstractions;
using Catalog.Domain;

namespace Catalog.Application.Products.GetProductById;

public class GetProductByIdHandler
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product?> HandleAsync(GetProductByIdQuery query, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(query.Id, cancellationToken);
        
        return product;
    }
}