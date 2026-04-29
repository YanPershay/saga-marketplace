namespace Catalog.Application.Products.CreateProduct;

public class CreateProductCommand
{
    public string Name { get; }
    public string Description { get; }
    public decimal Price { get; }
    public int QuantityAvailable { get; }
    public string Category { get; }

    public CreateProductCommand(string name, string description, decimal price, int quantityAvailable, string category)
    {
        Name = name;
        Description = description;
        Price = price;
        QuantityAvailable = quantityAvailable;
        Category = category;
    }
}