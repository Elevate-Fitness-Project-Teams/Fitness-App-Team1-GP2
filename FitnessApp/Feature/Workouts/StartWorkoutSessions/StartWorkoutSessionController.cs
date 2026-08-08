using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Common;
using FitnessApp.Common.Security;

namespace WorkoutService.Feature.Workouts.StartWorkoutSessions
{
    [ApiController]
    [Route("api/v1/workouts")]
    [Authorize]
    public class StartWorkoutSessionController(
        IValidator<StartWorkoutSessionCommand> validator,
        IMediator mediator,
        IUserContext userContext) : ControllerBase
    {
        [HttpPost("{id:int}/start")]
        public async Task<IActionResult> StartSession(
            [FromRoute] int id,
            [FromBody] StartWorkoutSessionRequest requestBody,
            CancellationToken cancellationToken)
        {

            var userId = userContext.UserId;


            var command = new StartWorkoutSessionCommand(
                id,
                userId,
                requestBody.Difficulty,
                requestBody.PlannedDuration
            );


            var validationResult =
                await validator.ValidateAsync(
                    command,
                    cancellationToken);


            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var response =
                    ApiResponse<StartWorkoutSessionResponse>
                    .Failure(
                        ErrorCode.ValidationError,
                        errors);

                return BadRequest(response);
            }


            var result =
                await mediator.Send(
                    command,
                    cancellationToken);


            if (!result.IsSuccess)
            {
                var response =
                    ApiResponse<StartWorkoutSessionResponse>
                    .Failure(result.ErrorCode);

                return StatusCode(
                    response.StatusCode,
                    response);
            }


            var responseSuccess =
                ApiResponse<StartWorkoutSessionResponse>
                .Success(
                    result.Data,
                    "Workout session started successfully.");


            return Ok(responseSuccess);
        }
    }
}