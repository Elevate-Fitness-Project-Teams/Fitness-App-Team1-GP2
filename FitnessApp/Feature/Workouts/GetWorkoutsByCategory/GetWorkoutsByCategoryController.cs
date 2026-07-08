using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Common;

namespace WorkoutService.Feature.Workouts.GetWorkoutsByCategory;

[ApiController]
[Route("api/v1/workouts")]
public sealed class GetWorkoutsByCategoryController(
    IMediator mediator,
    IValidator<GetWorkoutsByCategoryQuery> validator) : ControllerBase
{
    [HttpGet("category/{categoryName}")]
    public async Task<IActionResult> GetWorkoutsByCategory(
        [FromRoute] string categoryName,
        CancellationToken cancellationToken)
    {
        var request = new GetWorkoutsByCategoryQuery(categoryName);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(x => x.ErrorMessage)
                .ToList();

            var response = ApiResponse<List<GetWorkoutsByCategoryResponse>>
                .Failure(ErrorCode.ValidationError, errors);

            return BadRequest(response);
        }

        var result = await mediator.Send(request, cancellationToken);

        if (!result.IsSuccess)
        {
            var response = ApiResponse<List<GetWorkoutsByCategoryResponse>>
                .Failure(result.ErrorCode);

            return StatusCode(response.StatusCode, response);
        }

        var successResponse = ApiResponse<List<GetWorkoutsByCategoryResponse>>
            .Success(result.Data);

        return Ok(successResponse);
    }
}