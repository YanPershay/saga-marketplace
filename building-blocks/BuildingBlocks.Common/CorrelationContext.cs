namespace BuildingBlocks.Common;

public sealed class CorrelationContext : ICorrelationContext
{
    private static readonly AsyncLocal<Guid?> _correlationId = new();
    public Guid? CorrelationId => _correlationId.Value;

    public void Set(Guid correlationId)
    {
        if (correlationId == Guid.Empty)
            throw new ArgumentException("CorrelationId cannot be empty.", nameof(correlationId));
        
        if (_correlationId.Value.HasValue)
            throw new InvalidOperationException("CorrelationId is already set for the current context.");

        _correlationId.Value = correlationId;
    }
    
    public void Clear()
    {
        _correlationId.Value = null;
    }
}