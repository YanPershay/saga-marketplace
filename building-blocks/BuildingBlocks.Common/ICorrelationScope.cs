namespace BuildingBlocks.Common;

public interface ICorrelationScope
{
    IDisposable Begin();
}