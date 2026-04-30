namespace Catalog.Infrastructure.Options;

public sealed class AiServiceOptions
{
    public required string BaseUrl { get; set; }
    public int TimeoutSeconds { get; set; } = 15;
}