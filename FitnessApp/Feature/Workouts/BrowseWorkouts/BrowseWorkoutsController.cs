using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Common;
using WorkoutService.Features.Workouts.BrowseWorkouts;

namespace WorkoutService.Feature.Workouts.BrowseWorkouts;

[ApiController]
[Route("api/v1/workouts")]
public class BrowseWorkoutsController(
    IMediator mediator,
    IValidator<BrowseWorkoutsQuery> validator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> BrowseWorkouts(
        [FromQuery] BrowseWorkoutsQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(x => x.ErrorMessage)
                .ToList();
            var response = ApiResponse<PagedResult<BrowseWorkoutsResponse>>.Failure(ErrorCode.ValidationError, errors);

            return BadRequest(response);
        }

        var result = await mediator.Send(request, cancellationToken);

        if (!result.IsSuccess)
        {
            var response = ApiResponse<PagedResult<BrowseWorkoutsResponse>>
                .Failure(result.ErrorCode);

            return StatusCode(response.StatusCode, response);
        }

        var successResponse = ApiResponse<PagedResult<BrowseWorkoutsResponse>>
            .Success(result.Data);

        return Ok(successResponse);
    }
}