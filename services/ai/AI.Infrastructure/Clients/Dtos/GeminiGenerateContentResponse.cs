namespace AI.Infrastructure.Clients.Dtos;

public class GeminiGenerateContentResponse
{
    public IReadOnlyCollection<GeminiCandidateDto> Candidates { get; init; } = null!;
}

public class GeminiCandidateDto
{
    public GeminiContentDto Content { get; init; } = null!;
}