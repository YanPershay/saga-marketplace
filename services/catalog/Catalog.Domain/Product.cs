namespace Catalog.Domain;

public sealed class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int QuantityAvailable { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string Category { get; private set; } = null!;
    
    private Product() { }
    
    public Product(Guid id,
        string name, 
        string description, 
        decimal price, 
        int quantityAvailable,
        DateTimeOffset createdAt, 
        string category)
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

        if (createdAt == default)
            throw new ArgumentException("CreatedAt must be provided.", nameof(createdAt));
        
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Product category must be provided.", nameof(category));

        Id = id;
        Name = name;
        Description = description;
        Price = price;
        QuantityAvailable = quantityAvailable;
        CreatedAt = createdAt;
        Category = category;
    }
    
    public void Update(string name, string description, decimal price, int quantityAvailable, string category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name must be provided.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description must be provided.", nameof(description));

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Product price cannot be negative.");

        if (quantityAvailable < 0)
            throw new ArgumentOutOfRangeException(nameof(quantityAvailable), "Available quantity cannot be negative.");
        
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Product category must be provided.", nameof(category));

        Name = name;
        Description = description;
        Price = price;
        QuantityAvailable = quantityAvailable;
        Category = category;
    }
}