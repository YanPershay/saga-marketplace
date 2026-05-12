namespace Shipping.Domain.Shipments;

public sealed class Shipment
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    
    private Shipment()
    {}

    public static Shipment Create(Guid orderId)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
        }

        return new Shipment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Status = ShipmentStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void MarkCreated()
    {
        if (Status != ShipmentStatus.Pending)
            throw new InvalidOperationException("Cannot mark shipped status change.");
        
        Status = ShipmentStatus.Created;
    }
    
    public void MarkFailed()
    {
        if (Status != ShipmentStatus.Pending)
            throw new InvalidOperationException("Cannot mark shipped status change.");
        
        Status = ShipmentStatus.Failed;
    }
}