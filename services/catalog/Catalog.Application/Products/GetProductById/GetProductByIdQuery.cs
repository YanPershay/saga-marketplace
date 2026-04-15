namespace Catalog.Application.Products.GetProductById;

public class GetProductByIdQuery
{
    public Guid Id { get; }

    public GetProductByIdQuery(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Product id cannot be empty.", nameof(id));

        Id = id;
    }
}