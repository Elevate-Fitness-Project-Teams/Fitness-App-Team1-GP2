using FCEService.Common.ViewModels;
using FCEService.Features.AssignUserPlanHistory.commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FCEService.Features.AssignUserPlanHistory
{
    [Route("api/v1/fitness/plan-history")]
    [ApiController]
    public class AssignUserPlanHistoryEndpoint : ControllerBase
    {
        private readonly IMediator _mediator;
        public AssignUserPlanHistoryEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<EndPointResponse<bool>> AssignUserPlanHistory([FromBody] AssignUserPlanHistoryRequestViewModel request)
        { 
            var result = await _mediator.Send(new AssignUserPlanHistorycommand(request.UserId, request.ExternalPlanId, request.AssignedAt, request.EndedAt, request.ReasonForChange));
            if (!result.IsSuccess)
            {
                return EndPointResponse<bool>.Failure(result.Message ?? "Failed to assign user plan history.");
            }
            return EndPointResponse<bool>.Success(result.Data, "User plan history assigned successfully.");
        }
    }

    public record AssignUserPlanHistoryRequestViewModel(int UserId, int ExternalPlanId, DateTime AssignedAt, DateTime? EndedAt, string? ReasonForChange);
}
