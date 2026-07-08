using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Common;

namespace WorkoutService.Feature.WorkoutPlans.GetWorkoutPlanById;

[ApiController]
[Route("api/v1/workout-plans")]
public sealed class GetWorkoutPlanByIdController(
    IMediator mediator,
    IValidator<GetWorkoutPlanByIdQuery> validator) : ControllerBase
{
    [HttpGet("{planId}")]
    public async Task<IActionResult> GetWorkoutPlanById(
        [FromRoute] string planId,
        CancellationToken cancellationToken)
    {
        var request = new GetWorkoutPlanByIdQuery(planId);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(x => x.ErrorMessage)
                .ToList();

            var response = ApiResponse<GetWorkoutPlanByIdResponse>
                .Failure(ErrorCode.ValidationError, errors);

            return BadRequest(response);
        }

        var result = await mediator.Send(request, cancellationToken);

        if (!result.IsSuccess)
        {
            var response = ApiResponse<GetWorkoutPlanByIdResponse>
                .Failure(result.ErrorCode);

            return StatusCode(response.StatusCode, response);
        }

        var successResponse = ApiResponse<GetWorkoutPlanByIdResponse>
            .Success(result.Data);

        return Ok(successResponse);
    }
}