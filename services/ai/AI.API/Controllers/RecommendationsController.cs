using AI.API.Contracts.Requests;
using AI.API.Contracts.Responses;
using AI.API.Mappings;
using AI.Application.GetRecommendations;
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
        var command = request.ToCommand();

        var result = await handler.HandleAsync(command, cancellationToken);

        return Ok(result.ToResponse());
    }
}