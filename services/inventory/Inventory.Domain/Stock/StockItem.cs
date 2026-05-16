namespace Inventory.Domain.Stock;

public sealed class StockItem
{
    public Guid ProductId { get; private set; }
    public int QuantityAvailable { get; private set; }
    public int QuantityReserved { get; private set; }

    private StockItem()
    {
    }

    public StockItem(Guid productId, int quantityAvailable)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty.", nameof(productId));

        if (quantityAvailable < 0)
            throw new ArgumentException("QuantityAvailable cannot be negative.", nameof(quantityAvailable));

        ProductId = productId;
        QuantityAvailable = quantityAvailable;
        QuantityReserved = 0;
    }

    public bool CanReserve(int quantity)
    {
        if (quantity <= 0)
            return false;

        return QuantityAvailable >= quantity;
    }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.", nameof(quantity));

        if (!CanReserve(quantity))
            throw new InvalidOperationException("Not enough stock available.");

        QuantityAvailable -= quantity;
        QuantityReserved += quantity;
    }
    
    public void CommitReservation(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        if (QuantityReserved < quantity)
            throw new InvalidOperationException(
                $"Cannot commit {quantity} items. Reserved quantity is {QuantityReserved}.");

        QuantityReserved -= quantity;
    }
}