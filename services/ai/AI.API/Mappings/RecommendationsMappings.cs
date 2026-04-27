using AI.API.Contracts.Requests;
using AI.API.Contracts.Responses;
using AI.Application.GetRecommendations;
using AI.Domain.Products;

namespace AI.API.Mappings;

public static class RecommendationsMappings
{
    public static GetRecommendationsCommand ToCommand(this GetRecommendationsRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));
        
        return new GetRecommendationsCommand
        {
            CurrentProduct = new ProductContext(
                request.CurrentProduct.Id,
                request.CurrentProduct.Name,
                request.CurrentProduct.Description,
                request.CurrentProduct.Price,
                request.CurrentProduct.Category
            ),

            CandidateProducts = request.CandidateProducts
                .Select(p => new CandidateProduct(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.Category
                ))
                .ToList()
        };
    }

    public static GetRecommendationsResponse ToResponse(this RecommendationResult result)
    {
        if (result is null)
            throw new ArgumentNullException(nameof(result));
        
        return new GetRecommendationsResponse
        {
            Recommendations = result.Recommendations
                .Select(r => new RecommendationItemResponse(
                    r.ProductId,
                    r.Reason))
                .ToList(),
            Provider = result.Provider,
            Model = result.Model
        };
    }
}