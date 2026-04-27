namespace AI.Infrastructure.Clients.Dtos;

public class GeminiGenerateContentRequest
{
    public IReadOnlyCollection<GeminiContentDto> Contents { get; init; } = null!;
}

public class GeminiContentDto
{
    public IReadOnlyCollection<GeminiPartDto> Parts { get; init; } = null!;
}

public class GeminiPartDto
{
    public string Text { get; init; } = null!;
}