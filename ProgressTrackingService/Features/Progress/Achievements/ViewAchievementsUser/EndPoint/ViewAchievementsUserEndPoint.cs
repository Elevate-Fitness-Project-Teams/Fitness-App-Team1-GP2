using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using ProgressTrackingService.Common.Pagination;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.Achievements.ViewAchievementsUser.Query;
using ProgressTrackingService.Features.Progress.Achievements.ViewAchievementsUser.ViewModel;

namespace ProgressTrackingService.Features.Progress.Achievements.ViewAchievementsUser.EndPoint;

[ApiController]
[Route("/api/v1/progress/achievements/{userId}")]
public class ViewAchievementsUserEndPoint(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    // [Authorize]
    [OutputCache(Duration = 30)]
    public async Task<ActionResult<ApiResponse<PaginatedResult<ViewAchievementsUserViewModel>>>> GetAllAchievementsUsers([FromRoute] string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(userId)) 
            return Unauthorized(ApiResponse<PaginatedResult<ViewAchievementsUserViewModel>>.Failure("Unauthorized access. Please login again.", Common.Response.StatusCode.Unauthorized));
        

        if (!int.TryParse(userId, out var parsedUserId))
            return BadRequest(ApiResponse<PaginatedResult<ViewAchievementsUserViewModel>>.Failure("Invalid User ID format. Expected a number.", Common.Response.StatusCode.BadRequest));

        var result = await _mediator.Send(new ViewAchievementsUserQuery(parsedUserId, pageNumber, pageSize), cancellationToken);

        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                Common.Response.StatusCode.BadRequest => BadRequest(ApiResponse<ViewAchievementsUserViewModel>.Failure(result.Message, Common.Response.StatusCode.BadRequest)),
                Common.Response.StatusCode.NotFound => NotFound(ApiResponse<ViewAchievementsUserViewModel>.Failure(result.Message, Common.Response.StatusCode.NotFound)),
                _ => StatusCode(500, ApiResponse<ViewAchievementsUserViewModel>.Failure("An internal server error occurred while retrieving achievements.", Common.Response.StatusCode.InternalServerError))
            };
        }

        var achievementsUsersViewModel = new ViewAchievementsUserViewModel()
        {
            UserAchievements = result.Data.Data
        };
        
        var paginated = new PaginatedResult<ViewAchievementsUserViewModel>([achievementsUsersViewModel], result.Data.TotalCount, result.Data.CurrentPage, result.Data.PageSize);

        return Ok(ApiResponse<PaginatedResult<ViewAchievementsUserViewModel>>.Success(paginated, "User achievements retrieved successfully.", Common.Response.StatusCode.Success));
    }
}