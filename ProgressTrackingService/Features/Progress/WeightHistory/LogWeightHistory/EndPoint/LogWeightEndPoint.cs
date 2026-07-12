using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Command;
using ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Request;
using ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.ViewModel;

namespace ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.EndPoint;

[ApiController]
[Route("/api/v1/progress/weight")]
public class LogWeightEndPoint (IMediator mediator): ControllerBase
{
    private readonly IMediator _mediator = mediator;
    
    [HttpPost]
    // [Authorize]
    public async Task<ActionResult<ApiResponse<LogWeightViewModel>>> LogWeight([FromBody] LogWeightRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Request.Headers["UserId"];
        
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse<LogWeightViewModel>.Failure("Unauthorized access. Please login again.", Common.Response.StatusCode.Unauthorized));

        
        var result = await _mediator.Send(new LogWeightCommand(request, int.Parse(userId!)), cancellationToken);

        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                Common.Response.StatusCode.BadRequest => BadRequest(ApiResponse<LogWeightViewModel>.Failure(result.Message, Common.Response.StatusCode.BadRequest)),
                Common.Response.StatusCode.NotFound => NotFound(ApiResponse<LogWeightViewModel>.Failure(result.Message, Common.Response.StatusCode.NotFound)),
                _ => StatusCode(500, ApiResponse<LogWeightViewModel>.Failure("An internal server error occurred while retrieving achievements.", Common.Response.StatusCode.InternalServerError))
            };
        }
        
        var logWeightViewModel = new LogWeightViewModel()
        {
            Bmi = result.Data.Bmi,
            DifferenceFromPrevious = result.Data.DifferenceFromPrevious,
            TotalWeightLost = result.Data.TotalWeightLost,
        };
        
        return Ok(ApiResponse<LogWeightViewModel>.Success(logWeightViewModel, "Weight logged and metrics updated successfully.", Common.Response.StatusCode.Success));
    }
}