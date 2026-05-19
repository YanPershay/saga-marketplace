using AI.API.Contracts.Requests;
using AI.API.Contracts.Responses;
using AI.API.Mappings;
using AI.Application.GetRecommendations;
using AI.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AI.API.Controllers;

[ApiController]
[Route("recommendations")]
public class RecommendationsController(
    GetRecommendationsHandler handler) : ControllerBase
{
    
    [HttpPost]
    public async Task<ActionResult<GetRecommendationsResponse>> GetRecommendations(
        [FromBody] GetRecommendationsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = request.ToCommand();

            var result = await handler.HandleAsync(command, cancellationToken);

            return Ok(result.ToResponse());
        }
        catch (AiProviderRateLimitException)
        {

            return StatusCode(
                StatusCodes.Status429TooManyRequests,
                new
                {
                    Error = "AI provider rate limit exceeded.",
                    Retryable = true
                });
        }
    }
}