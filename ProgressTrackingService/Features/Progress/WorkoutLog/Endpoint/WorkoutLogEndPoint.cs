using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.WorkoutLog.Command;
using ProgressTrackingService.Features.Progress.WorkoutLog.Request;
using ProgressTrackingService.Features.Progress.WorkoutLog.ViewModel;

namespace ProgressTrackingService.Features.Progress.WorkoutLog.Endpoint;

[ApiController]
[Route("api/v1/progress/workouts")]
public class WorkoutLogEndPoint (IMediator mediator): ControllerBase
{
    private IMediator Mediator { get; set; } = mediator;
    
    [HttpPost]
    // [Authorize]
    public async Task<ActionResult<ApiResponse<WorkoutLogViewModel>>> WorkoutLog([FromBody] WorkoutLogRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Request.Headers["UserId"];
        
        if  (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse<WorkoutLogViewModel>.Failure("Unauthorized access. Please login again.", Common.Response.StatusCode.Unauthorized));

        if (!int.TryParse(userId, out var parsedUserId))
            return BadRequest(ApiResponse<WorkoutLogViewModel>.Failure("Invalid User ID format. Expected a number.", Common.Response.StatusCode.BadRequest));
        
        var result = await Mediator.Send(new WorkoutLogCommand(parsedUserId, request), cancellationToken);

        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                Common.Response.StatusCode.BadRequest => BadRequest(ApiResponse<WorkoutLogViewModel>.Failure(result.Message, Common.Response.StatusCode.BadRequest)),
                Common.Response.StatusCode.NotFound => NotFound(ApiResponse<WorkoutLogViewModel>.Failure(result.Message, Common.Response.StatusCode.NotFound)),
                _ => StatusCode(500, ApiResponse<WorkoutLogViewModel>.Failure("An internal server error occurred while retrieving achievements.", Common.Response.StatusCode.InternalServerError))
            };
        }

        var workoutLogViewModel = new WorkoutLogViewModel()
        {
            WorkoutLogLogId = result.Data.WorkoutLogLogId,
            CurrentStreak = result.Data.CurrentStreak,
            StreakUpdated = result.Data.StreakUpdated
        };
        
        return Ok(ApiResponse<WorkoutLogViewModel>.Success(workoutLogViewModel, "Workout logged and activity streak updated successfully.", Common.Response.StatusCode.Success));
    }
}