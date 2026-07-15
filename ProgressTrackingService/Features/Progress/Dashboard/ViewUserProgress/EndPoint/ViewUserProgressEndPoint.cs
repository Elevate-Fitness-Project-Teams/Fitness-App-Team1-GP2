using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.Dashboard.ViewUserProgress.Query;
using ProgressTrackingService.Features.Progress.Dashboard.ViewUserProgress.ViewModel;

namespace ProgressTrackingService.Features.Progress.Dashboard.ViewUserProgress.EndPoint;

[ApiController]
[Route("/api/v1/progress/{userId}")]
public class ViewUserProgressEndPoint (IMediator mediator): ControllerBase
{
    private readonly IMediator _mediator = mediator;
    
    [HttpGet]
    // [Authorize]
    public async Task<ActionResult<ApiResponse<ViewUserProgressViewModel>>> ViewUserProgress([FromRoute] string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse<ViewUserProgressViewModel>.Failure("Unauthorized access. Please login again.", Common.Response.StatusCode.Unauthorized));

        
        if (!int.TryParse(userId, out var parsedUserId))
            return BadRequest(ApiResponse<ViewUserProgressViewModel>.Failure("Invalid User ID format. Expected a number.", Common.Response.StatusCode.BadRequest));
        
        var result = await _mediator.Send(new ViewUserProgressQuery(parsedUserId), cancellationToken);

        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                Common.Response.StatusCode.BadRequest => BadRequest(ApiResponse<ViewUserProgressViewModel>.Failure(result.Message, Common.Response.StatusCode.BadRequest)),
                Common.Response.StatusCode.NotFound => NotFound(ApiResponse<ViewUserProgressViewModel>.Failure(result.Message, Common.Response.StatusCode.NotFound)),
                _ => StatusCode(500, ApiResponse<ViewUserProgressViewModel>.Failure("An internal server error occurred while retrieving achievements.", Common.Response.StatusCode.InternalServerError))
            };
        }
        
        var viewWeightViewModel = new ViewUserProgressViewModel()
        {
            WorkoutLogs = result.Data.WorkoutLogs,
            WeightHistory = result.Data.WeightHistory,
            UserStatistics = result.Data.UserStatistics
        };
        
        return Ok(ApiResponse<ViewUserProgressViewModel>.Success(viewWeightViewModel, "User progress history retrieved successfully.", Common.Response.StatusCode.Success));
    }
}