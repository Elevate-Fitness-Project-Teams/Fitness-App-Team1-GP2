using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkoutService.Common;

namespace WorkoutService.Feature.Workouts.StartWorkoutSessions
{
    [ApiController]
    [Route("api/v1/workouts")]
    [Authorize]
    public class StartWorkoutSessionController(
        IValidator<StartWorkoutSessionCommand> validator,
        IMediator mediator) : ControllerBase
    {
        [HttpPost("{id:int}/start")]
        public async Task<IActionResult> StartSession(
            [FromRoute] int id,
            [FromBody] StartWorkoutSessionRequest requestBody,
            CancellationToken cancellationToken)
        {

            var userIdValue = 
           User.FindFirst(ClaimTypes.NameIdentifier)?.Value
           ?? User.FindFirst("sub")?.Value
           ?? User.FindFirst("userId")?.Value;

            if (!int.TryParse(userIdValue, out var userId))
            {
                var response = ApiResponse<StartWorkoutSessionResponse>
                    .Failure(ErrorCode.Unauthorized);

                return Unauthorized(response);
            }

            var request = new StartWorkoutSessionCommand(
                id,
                userId,
                requestBody.Difficulty,
                requestBody.PlannedDuration);

            var command = new StartWorkoutSessionCommand(
                id,
                userId,
                requestBody.Difficulty,
                requestBody.PlannedDuration
            );

            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var response = ApiResponse<StartWorkoutSessionResponse>
                    .Failure(ErrorCode.ValidationError, errors);

                return BadRequest(response);
            }

            var result = await mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                var response = ApiResponse<StartWorkoutSessionResponse>
                    .Failure(result.ErrorCode);

                return StatusCode(response.StatusCode, response);
            }

            var responseSuccess = ApiResponse<StartWorkoutSessionResponse>
                .Success(result.Data, "Workout session started successfully.");

            return Ok(responseSuccess);
        }
    }
}