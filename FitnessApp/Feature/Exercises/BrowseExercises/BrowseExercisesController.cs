using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Common;

namespace WorkoutService.Feature.Exercises.BrowseExercises;

[ApiController]
[Route("api/v1/exercises")]
public sealed class BrowseExercisesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> BrowseExercises(
     [FromQuery] BrowseExercisesQuery request,
     CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);

        if (!result.IsSuccess)
        {
            var response = ApiResponse<PagedResult<BrowseExercisesResponse>>
                .Failure(result.ErrorCode);

            return StatusCode(response.StatusCode, response);
        }

        var successResponse = ApiResponse<PagedResult<BrowseExercisesResponse>>
            .Success(result.Data);

        return Ok(successResponse);
    }
}