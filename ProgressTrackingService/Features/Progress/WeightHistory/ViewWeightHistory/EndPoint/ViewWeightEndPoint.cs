using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackingService.Common.Pagination;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.WeightHistory.ViewWeightHistory.Query;
using ProgressTrackingService.Features.Progress.WeightHistory.ViewWeightHistory.ViewModel;

namespace ProgressTrackingService.Features.Progress.WeightHistory.ViewWeightHistory.EndPoint;

[ApiController]
[Route("api/v1/progress/weight-history")]
public class ViewWeightEndPoint(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<ApiResponse<PaginatedResult<ViewWeightViewModel>>>> GetWeightHistory(int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ViewWeightQuery(userId, pageNumber, pageSize), cancellationToken);

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
            WeightHistory = result.Data.Data
        };
        
        var paginated = new PaginatedResult<ViewWeightViewModel>([viewModel], result.Data.TotalCount, result.Data.CurrentPage, result.Data.PageSize);
        
        return Ok(ApiResponse<PaginatedResult<ViewWeightViewModel>>.Success(paginated, result.Message, Common.Response.StatusCode.Success));    }
}