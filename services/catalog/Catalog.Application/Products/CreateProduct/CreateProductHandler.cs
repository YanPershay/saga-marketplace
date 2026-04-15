using Catalog.Application.Abstractions;
using Catalog.Domain;

namespace Catalog.Application.Products.CreateProduct;

public class CreateProductHandler
{
        private readonly IProductRepository _productRepository;
    
        public CreateProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
    
        public async Task<Guid> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
        {
            var productId = Guid.NewGuid();
            var newProduct = new Product(
                productId,
                command.Name,
                command.Description,
                command.Price,
                command.QuantityAvailable,
                DateTimeOffset.UtcNow
            );
        
            await _productRepository.AddAsync(newProduct, cancellationToken);
            
            return productId;
        }
}