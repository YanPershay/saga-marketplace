namespace Catalog.Application.Products.DeleteProduct;

public class DeleteProductCommand
{
    public Guid Id { get; }

    public DeleteProductCommand(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Product id cannot be empty.", nameof(id));
        
        Id = id;
    }
}