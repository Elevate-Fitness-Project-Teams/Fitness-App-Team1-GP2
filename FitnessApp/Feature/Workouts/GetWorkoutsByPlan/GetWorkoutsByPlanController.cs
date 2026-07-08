using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Common;

namespace WorkoutService.Feature.Workouts.GetWorkoutsByPlan;

[ApiController]
[Route("api/v1/workouts")]
public sealed class GetWorkoutsByPlanController(
    IMediator mediator,
    IValidator<GetWorkoutsByPlanQuery> validator) : ControllerBase
{
    [HttpGet("by-plan/{planId}")]
    public async Task<IActionResult> GetWorkoutsByPlan(
        [FromRoute] string planId,
        CancellationToken cancellationToken)
    {
        var request = new GetWorkoutsByPlanQuery(planId);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(x => x.ErrorMessage)
                .ToList();

            var response = ApiResponse<List<GetWorkoutsByPlanResponse>>
                .Failure(ErrorCode.ValidationError, errors);

            return BadRequest(response);
        }

        var result = await mediator.Send(request, cancellationToken);

        if (!result.IsSuccess)
        {
            var response = ApiResponse<List<GetWorkoutsByPlanResponse>>
                .Failure(result.ErrorCode);

            return StatusCode(response.StatusCode, response);
        }

        var successResponse = ApiResponse<List<GetWorkoutsByPlanResponse>>
            .Success(result.Data);

        return Ok(successResponse);
    }
}