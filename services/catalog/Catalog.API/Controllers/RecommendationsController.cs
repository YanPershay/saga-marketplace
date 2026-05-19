using Catalog.API.Contracts.Requests;
using Catalog.API.Messaging;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Catalog.Infrastructure.Persistence.Cosmos.Storage;

namespace Catalog.API.Controllers;

[ApiController]
[Route("products/{productId:guid}/recommendations")]
public sealed class RecommendationsController : ControllerBase
{
    private readonly CosmosRecommendationReader _recommendationReader;
    
    public RecommendationsController(CosmosRecommendationReader recommendationReader)
    {
        _recommendationReader = recommendationReader;
    }
    
    [HttpPost]
    public async Task<IActionResult> RequestRecommendationsAsync(
        [FromBody] RequestRecommendationsRequest request,
        [FromServices] RecommendationRequestedPublisher publisher,
        CancellationToken cancellationToken)
    {
        var correlationId = HttpContext.TraceIdentifier;

        var currentProduct = new ProductContextMessage(
            request.CurrentProduct.Id,
            request.CurrentProduct.Name,
            request.CurrentProduct.Description,
            request.CurrentProduct.Price,
            request.CurrentProduct.Category);

        var candidateProducts = request.CandidateProducts
            .Select(x => new CandidateProductMessage(
                x.Id,
                x.Name,
                x.Description,
                x.Price,
                x.Category))
            .ToList();

        var requestId = await publisher.PublishAsync(
            currentProduct,
            candidateProducts,
            correlationId,
            cancellationToken);

        return Accepted(
            $"/products/{currentProduct.Id}/recommendations/async/{requestId}",
            new
            {
                RequestId = requestId,
                Status = "Processing"
            });
    }
    
    [HttpGet("{requestId}")]
    public async Task<IActionResult> GetAsync(
        string requestId,
        CancellationToken cancellationToken)
    {
        var result = await _recommendationReader.GetAsync(
            requestId,
            cancellationToken);

        if (result is null)
        {
            return Ok(new
            {
                RequestId = requestId,
                Status = "Processing"
            });
        }

        return Ok(result);
    }
}