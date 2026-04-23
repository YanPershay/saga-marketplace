namespace AI.Infrastructure.Options;

public class GeminiOptions
{
    public required string ApiKey { get; set; }
    public required string BaseUrl { get; set; }
    public required string Model { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
}