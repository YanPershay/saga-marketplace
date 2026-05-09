namespace Order.Domain.Orders;

public sealed class Order
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; set; }
    public decimal TotalPrice { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    
    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;
    
    private Order() { }
    
    public static Order Create(Guid customerId, IReadOnlyCollection<OrderItem> orderItems)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty.", nameof(customerId));
        
        if (orderItems == null || !orderItems.Any())
            throw new ArgumentException("Order must contain at least one item.", nameof(orderItems));

        var totalPrice = orderItems.Sum(item => item.Price * item.Quantity);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Status = OrderStatus.AwaitingInventory,
            TotalPrice = totalPrice,
            CreatedAt = DateTimeOffset.UtcNow
        };
        
        order._orderItems.AddRange(orderItems);
        
        return order;
    }
    
    public void MarkInventoryReserved()
    {
        if (Status != OrderStatus.AwaitingInventory)
            throw new InvalidOperationException(
                $"Cannot mark inventory as reserved when order status is {Status}.");

        Status = OrderStatus.InventoryReserved;
    }

    public void MarkAwaitingPayment()
    {
        if (Status != OrderStatus.InventoryReserved)
            throw new InvalidOperationException(
                $"Cannot mark order as awaiting payment when order status is {Status}.");

        Status = OrderStatus.AwaitingPayment;
    }

    public void MarkPaymentSucceeded()
    {
        if (Status != OrderStatus.AwaitingPayment)
            throw new InvalidOperationException(
                $"Cannot mark payment as succeeded when order status is {Status}.");

        Status = OrderStatus.PaymentSucceeded;
    }

    public void MarkPaymentFailed()
    {
        if (Status != OrderStatus.AwaitingPayment)
            throw new InvalidOperationException(
                $"Cannot mark payment as failed when order status is {Status}.");

        Status = OrderStatus.PaymentFailed;
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Completed or OrderStatus.Cancelled)
            throw new InvalidOperationException(
                $"Cannot cancel order when order status is {Status}.");

        Status = OrderStatus.Cancelled;
    }
}