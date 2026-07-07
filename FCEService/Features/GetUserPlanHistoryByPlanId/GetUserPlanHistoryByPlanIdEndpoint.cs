using FCEService.Common.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FCEService.Features.GetUserPlanHistoryByPlanId
{
    [Route("api/v1/fitness/plans/")]
    [ApiController]
    public class GetUserPlanHistoryByPlanIdEndpoint : ControllerBase
    {
        private readonly IMediator _mediator;

        public GetUserPlanHistoryByPlanIdEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{planId}")]
        public async Task<EndPointResponse<List<UsersPlanHistoryResponseViewModel>>> GetUsersPlanHistories(int planId)
        {
            var result = await _mediator.Send(new Queries.GetUserPlanHistoryByPlanIdQuery(planId));
            if (!result.IsSuccess)
            {
                return EndPointResponse<List<UsersPlanHistoryResponseViewModel>>.Failure(result.Message ?? "Failed to retrieve user plan histories.");
            }
            var responseData = result.Data?.Select(dto => new UsersPlanHistoryResponseViewModel(
                dto.UserId,
                dto.ExternalPlanId,
                dto.AssignedAt,
                dto.EndedAt,
                dto.ResonForChange
            )).ToList();
            return EndPointResponse<List<UsersPlanHistoryResponseViewModel>>.Success(responseData, "User plan histories retrieved successfully.");
        }
    }
    public record UsersPlanHistoryResponseViewModel(int UserId, int ExternalPlanId, DateTime AssignedAt, DateTime? EndedAt, string? ReasonForChange);
}

    