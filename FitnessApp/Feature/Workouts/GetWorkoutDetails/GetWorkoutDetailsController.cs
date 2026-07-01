using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Common;

namespace WorkoutService.Feature.Workouts.GetWorkoutDetails;

[ApiController]
[Route("api/v1/workouts")]
public sealed class GetWorkoutDetailsController(
    IMediator mediator,
    IValidator<GetWorkoutDetailsQuery> validator) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWorkoutDetails(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var request = new GetWorkoutDetailsQuery(id);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(x => x.ErrorMessage)
                .ToList();

            var response = ApiResponse<GetWorkoutDetailsResponse>
                .Failure(ErrorCode.ValidationError, errors);

            return BadRequest(response);
        }

        var result = await mediator.Send(request, cancellationToken);

        if (!result.IsSuccess)
        {
            var response = ApiResponse<GetWorkoutDetailsResponse>
                .Failure(result.ErrorCode);

            return StatusCode(response.StatusCode, response);
        }

        var successResponse = ApiResponse<GetWorkoutDetailsResponse>
            .Success(result.Data);

        return Ok(successResponse);
    }
}