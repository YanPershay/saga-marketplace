namespace AI.API.Contracts.Requests;

public sealed record CandidateProductRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public decimal Price { get; init; }
    public string Category { get; init; } = null!;
}