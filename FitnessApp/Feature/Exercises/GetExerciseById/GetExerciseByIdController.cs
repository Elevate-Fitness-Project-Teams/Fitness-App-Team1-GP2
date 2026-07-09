using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Common;

namespace WorkoutService.Feature.Exercises.GetExerciseById;

[ApiController]
[Route("api/v1/exercises")]
public sealed class GetExerciseByIdController(
    IMediator mediator,
    IValidator<GetExerciseByIdQuery> validator) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetExerciseById(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var request = new GetExerciseByIdQuery(id);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(x => x.ErrorMessage)
                .ToList();

            var response = ApiResponse<GetExerciseByIdResponse>
                .Failure(ErrorCode.ValidationError, errors);

            return BadRequest(response);
        }

        var result = await mediator.Send(request, cancellationToken);

        if (!result.IsSuccess)
        {
            var response = ApiResponse<GetExerciseByIdResponse>
                .Failure(result.ErrorCode);

            return StatusCode(response.StatusCode, response);
        }

        var successResponse = ApiResponse<GetExerciseByIdResponse>
            .Success(result.Data);

        return Ok(successResponse);
    }
}