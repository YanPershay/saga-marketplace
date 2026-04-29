using Catalog.Application.Abstractions;

namespace Catalog.Application.Products.UpdateProduct;

public class UpdateProductHandler
{
    private readonly IProductRepository _productRepository;

    public UpdateProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task HandleAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with id {command.Id} not found.");
        
        product.Update(command.Name, command.Description, command.Price, command.QuantityAvailable, command.Category);
        
        await _productRepository.UpdateAsync(product, cancellationToken);
    }
}