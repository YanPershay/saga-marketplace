namespace Order.Domain.Orders;

public sealed record OrderItem
{
    public Guid ProductId { get; }
    public int Quantity { get; }
    public decimal Price { get; }

    public OrderItem(Guid productId, int quantity, decimal price)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty.", nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.", nameof(quantity));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));

        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }
}