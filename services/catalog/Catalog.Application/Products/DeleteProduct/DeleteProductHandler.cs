using Catalog.Application.Abstractions;

namespace Catalog.Application.Products.DeleteProduct;

public class DeleteProductHandler
{
    private readonly IProductRepository _productRepository;

    public DeleteProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task HandleAsync(DeleteProductCommand command, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with id {command.Id} not found.");
        
        await _productRepository.DeleteAsync(product, cancellationToken);
    }
}