using AI.Application.Abstractions;
using AI.Domain.Products;

namespace AI.Application.GetRecommendations;

public class GetRecommendationsHandler
{
    private readonly IRecommendationProvider _recommendationProvider;
    
    public GetRecommendationsHandler(IRecommendationProvider recommendationProvider)
    {
        _recommendationProvider = recommendationProvider ?? throw new ArgumentNullException(nameof(recommendationProvider));
    }
    
    public async Task<RecommendationResult> HandleAsync(
        GetRecommendationsCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.CurrentProduct == null)
        {
            throw new ArgumentNullException(nameof(command.CurrentProduct));
        }
        
        if (command.CandidateProducts == null || !command.CandidateProducts.Any())
        {
            throw new ArgumentException("Candidate products cannot be null or empty.", nameof(command.CandidateProducts));
        }
        
        // TODO: move max candidates to configuration
        int maxCandidates = 20;

        var candidates = command.CandidateProducts
            .Where(c => c.Id != command.CurrentProduct.Id)
            .Take(maxCandidates)
            .ToList();

        return await _recommendationProvider.GetRecommendationsAsync(
            command.CurrentProduct,
            candidates,
            cancellationToken);
    }
}