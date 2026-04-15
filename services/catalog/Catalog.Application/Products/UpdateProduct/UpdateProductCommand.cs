namespace Catalog.Application.Products.UpdateProduct;

public class UpdateProductCommand
{
    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public decimal Price { get; }
    
    public int QuantityAvailable { get; }
    
    public UpdateProductCommand(Guid id, string name, string description, decimal price, int quantityAvailable)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Product id cannot be empty.", nameof(id));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name must be provided.", nameof(name));
        
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description must be provided.", nameof(description));
        
        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Product price cannot be negative.");
        
        if (quantityAvailable < 0)
            throw new ArgumentOutOfRangeException(nameof(quantityAvailable), "Available quantity cannot be negative.");

        Id = id;
        Name = name;
        Description = description;
        Price = price;
        QuantityAvailable = quantityAvailable;
    }
}