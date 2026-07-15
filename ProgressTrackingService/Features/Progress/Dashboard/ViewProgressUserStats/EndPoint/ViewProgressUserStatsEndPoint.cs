using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.Dashboard.ViewProgressUserStats.Query;
using ProgressTrackingService.Features.Progress.Dashboard.ViewProgressUserStats.ViewModel;

namespace ProgressTrackingService.Features.Progress.Dashboard.ViewProgressUserStats.EndPoint;

[ApiController]
[Route("/api/v1/progress/stats/{userId}")]
public class ViewProgressUserStatsEndPoint (IMediator mediator): ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    // [Authorize]
    public async Task<ActionResult<ApiResponse<ViewProgressUserStatsViewModel>>> ViewProgressUserStats([FromRoute] string userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse<ViewProgressUserStatsViewModel>.Failure("Unauthorized access. Please login again.", Common.Response.StatusCode.Unauthorized));

            
        if (!int.TryParse(userId, out int parsedUserId))
            return BadRequest(ApiResponse<ViewProgressUserStatsQuery>.Failure("Invalid User ID format. Expected a number.", Common.Response.StatusCode.BadRequest));
        
        var result = await _mediator.Send(new ViewProgressUserStatsQuery(parsedUserId), cancellationToken);

        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                Common.Response.StatusCode.BadRequest => BadRequest(ApiResponse<ViewProgressUserStatsQuery>.Failure(result.Message, Common.Response.StatusCode.BadRequest)),
                Common.Response.StatusCode.NotFound => NotFound(ApiResponse<ViewProgressUserStatsQuery>.Failure(result.Message, Common.Response.StatusCode.NotFound)),
                _ => StatusCode(500, ApiResponse<ViewProgressUserStatsQuery>.Failure("An internal server error occurred while retrieving achievements.", Common.Response.StatusCode.InternalServerError))
            };
        }

        var viewProgressUserStatsViewModel = new ViewProgressUserStatsViewModel()
        {
            UserId = result.Data.UserId,
            TotalCaloriesBurned = result.Data.TotalCaloriesBurned,
            TotalWorkouts = result.Data.TotalWorkouts,
            TotalWeightLost = result.Data.TotalWeightLost,
            CurrentStreak = result.Data.CurrentStreak,
            LongestStreak = result.Data.LongestStreak,
        };
        
        return Ok(ApiResponse<ViewProgressUserStatsViewModel>.Success(viewProgressUserStatsViewModel, "User statistics retrieved successfully.", Common.Response.StatusCode.Success));
    }
}