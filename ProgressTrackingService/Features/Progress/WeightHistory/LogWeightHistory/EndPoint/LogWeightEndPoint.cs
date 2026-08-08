using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Command;
using ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Request;
using ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.ViewModel;

namespace ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.EndPoint;

[ApiController]
[Route("/api/v1/progress/weight/{userId}")]
public class LogWeightEndPoint (IMediator mediator): ControllerBase
{
    private readonly IMediator _mediator = mediator;
    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<LogWeightViewModel>>> LogWeight([FromRoute] string userId, [FromBody] LogWeightRequest request, CancellationToken cancellationToken)
    {
        var userIdFromToken = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdFromToken) || userIdFromToken != userId)
            return Unauthorized(ApiResponse<LogWeightViewModel>.Failure("Unauthorized access. Please login again.", Common.Response.StatusCode.Unauthorized));

        if (!int.TryParse(userIdFromToken, out var parsedUserId))
            return BadRequest(ApiResponse<LogWeightViewModel>.Failure("Invalid User ID format. Expected a number.", Common.Response.StatusCode.BadRequest));

        var result = await _mediator.Send(new LogWeightCommand(request, parsedUserId), cancellationToken);

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