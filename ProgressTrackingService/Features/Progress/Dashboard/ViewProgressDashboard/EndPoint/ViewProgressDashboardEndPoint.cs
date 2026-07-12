using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.Dashboard.ViewProgressDashboard.Query;
using ProgressTrackingService.Features.Progress.Dashboard.ViewProgressDashboard.ViewModel;

namespace ProgressTrackingService.Features.Progress.Dashboard.ViewProgressDashboard.EndPoint;

[ApiController]
[Route("/api/v1/progress")]
public class ViewProgressDashboardEndPoint (IMediator mediator): ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    // [Authorize]
    public async Task<ActionResult<ApiResponse<ViewProgressDashboardViewModel>>> ViewProgressDashboard(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ViewProgressDashboardQuery(), cancellationToken);

        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                Common.Response.StatusCode.BadRequest => BadRequest(ApiResponse<ViewProgressDashboardViewModel>.Failure(result.Message, Common.Response.StatusCode.BadRequest)),
                Common.Response.StatusCode.NotFound => BadRequest(ApiResponse<ViewProgressDashboardViewModel>.Failure(result.Message, Common.Response.StatusCode.NotFound)),
                _ => StatusCode(500, ApiResponse<ViewProgressDashboardViewModel>.Failure("An internal server error occurred while retrieving achievements.", Common.Response.StatusCode.InternalServerError))
            };
        }

        var viewProgressDashboardViewModel = new ViewProgressDashboardViewModel()
        {
            WeightHistory = result.Data.WeightHistory,
            WorkoutLogsHistory = result.Data.WorkoutLogsHistory,
            UserAchievements = result.Data.UserAchievements
        };
        
        return Ok(ApiResponse<ViewProgressDashboardViewModel>.Success(viewProgressDashboardViewModel, "Progress dashboard data retrieved successfully.", Common.Response.StatusCode.Success));
    }
}