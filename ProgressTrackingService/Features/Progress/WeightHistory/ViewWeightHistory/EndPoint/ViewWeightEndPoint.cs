using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackingService.Common.Pagination;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.ViewModel;
using ProgressTrackingService.Features.Progress.WeightHistory.ViewWeightHistory.Query;
using ProgressTrackingService.Features.Progress.WeightHistory.ViewWeightHistory.ViewModel;

namespace ProgressTrackingService.Features.Progress.WeightHistory.ViewWeightHistory.EndPoint;

[ApiController]
[Route("api/v1/progress/weight-history")]
public class ViewWeightEndPoint(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [Authorize]
    [HttpGet("{userId}")]
    public async Task<ActionResult<ApiResponse<PaginatedResult<ViewWeightViewModel>>>> GetWeightHistory([FromRoute] string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var userIdFromToken = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdFromToken))
            return Unauthorized(ApiResponse<LogWeightViewModel>.Failure("Unauthorized access. Please login again.", Common.Response.StatusCode.Unauthorized));

        if (!int.TryParse(userIdFromToken, out var parsedUserId))
            return BadRequest(ApiResponse<LogWeightViewModel>.Failure("Invalid User ID format. Expected a number.", Common.Response.StatusCode.BadRequest));
        
        var result = await _mediator.Send(new ViewWeightQuery(parsedUserId, pageNumber, pageSize), cancellationToken);

        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                Common.Response.StatusCode.BadRequest => BadRequest(ApiResponse<object>.Failure(result.Message, Common.Response.StatusCode.BadRequest)),
                Common.Response.StatusCode.NotFound => NotFound(ApiResponse<object>.Failure(result.Message, Common.Response.StatusCode.NotFound)),
                _ => StatusCode(500, ApiResponse<object>.Failure("An unexpected error occurred.", Common.Response.StatusCode.InternalServerError))
            };
        }

        var viewModel = new ViewWeightViewModel
        {
            WeightHistory = result.Data.Data!
        };
        
        var paginated = new PaginatedResult<ViewWeightViewModel>([viewModel], result.Data.TotalCount, result.Data.CurrentPage, result.Data.PageSize);
        
        return Ok(ApiResponse<PaginatedResult<ViewWeightViewModel>>.Success(paginated, result.Message, Common.Response.StatusCode.Success));    }
}