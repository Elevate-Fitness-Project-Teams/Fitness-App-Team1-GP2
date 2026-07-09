using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Common;

namespace WorkoutService.Feature.WorkoutPlans.BrowseWorkoutPlans;

[ApiController]
[Route("api/v1/workout-plans")]
public sealed class BrowseWorkoutPlansController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> BrowseWorkoutPlans(
        CancellationToken cancellationToken)
    {
        var request = new BrowseWorkoutPlansQuery();

        var result = await mediator.Send(request, cancellationToken);

        if (!result.IsSuccess)
        {
            var response = ApiResponse<List<BrowseWorkoutPlansResponse>>
                .Failure(result.ErrorCode);

            return StatusCode(response.StatusCode, response);
        }

        var successResponse = ApiResponse<List<BrowseWorkoutPlansResponse>>
            .Success(result.Data);

        return Ok(successResponse);
    }
}