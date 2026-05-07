namespace Payment.Domain.Payments;

public sealed class Payment
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Payment()
    {
    }

    public static Payment Create(Guid orderId, decimal amount)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("OrderId cannot be empty.", nameof(orderId));

        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));

        return new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Amount = amount,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void MarkAsSucceeded()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payment can be succeeded.");

        Status = PaymentStatus.Succeeded;
    }

    public void MarkAsFailed()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payment can be failed.");

        Status = PaymentStatus.Failed;
    }
}