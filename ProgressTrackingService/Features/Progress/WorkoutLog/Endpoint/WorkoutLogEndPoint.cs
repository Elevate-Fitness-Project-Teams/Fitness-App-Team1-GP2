using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.WorkoutLog.Command;
using ProgressTrackingService.Features.Progress.WorkoutLog.Request;
using ProgressTrackingService.Features.Progress.WorkoutLog.ViewModel;

namespace ProgressTrackingService.Features.Progress.WorkoutLog.Endpoint;

[ApiController]
[Route("api/v1/progress/workouts")]
public class WorkoutLogEndPoint(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<WorkoutLogViewModel>>> WorkoutLog(
        [FromBody] WorkoutLogRequest request, 
        CancellationToken cancellationToken)
    {
        var userIdStr = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            return Unauthorized(ApiResponse<WorkoutLogViewModel>.Failure("Unauthorized access. Please login again.", Common.Response.StatusCode.Unauthorized));
        
        var result = await _mediator.Send(new WorkoutLogCommand(userId, request), cancellationToken);

        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                Common.Response.StatusCode.BadRequest => BadRequest(ApiResponse<WorkoutLogViewModel>.Failure(result.Message, Common.Response.StatusCode.BadRequest)),
                Common.Response.StatusCode.NotFound => NotFound(ApiResponse<WorkoutLogViewModel>.Failure(result.Message, Common.Response.StatusCode.NotFound)),
                _ => StatusCode(500, ApiResponse<WorkoutLogViewModel>.Failure("An internal server error occurred.", Common.Response.StatusCode.InternalServerError))
            };
        }

        var workoutLogViewModel = new WorkoutLogViewModel
        {
            WorkoutLogLogId = result.Data.WorkoutLogLogId,
            CurrentStreak = result.Data.CurrentStreak,
            StreakUpdated = result.Data.StreakUpdated
        };
        
        return Ok(ApiResponse<WorkoutLogViewModel>.Success(workoutLogViewModel, "Workout logged successfully.", Common.Response.StatusCode.Success));
    }
}