namespace BuildingBlocks.Common;

public interface ICorrelationContext
{
    Guid? CorrelationId { get; }
    void Set(Guid correlationId);
    void Clear();
}