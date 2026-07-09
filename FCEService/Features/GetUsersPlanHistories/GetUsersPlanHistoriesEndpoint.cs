using FCEService.Common.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FCEService.Features.GetUsersPlanHistories
{
    [Route("api/v1/fitness/plan-configs")]
    [ApiController]
    public class GetUserPlanHistoryByPlanIdEndpoint : ControllerBase
    {
        private readonly IMediator mediator;

        public GetUserPlanHistoryByPlanIdEndpoint(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<EndPointResponse<List<UsersPlanHistoriesResponseViewModel>>> GetUsersPlanHistories()
        {
            var result = await mediator.Send(new Queries.GetUserPlanHistoryByPlanIdQuery());
            if (!result.IsSuccess)
            {
                return EndPointResponse<List<UsersPlanHistoriesResponseViewModel>>.Failure(result.Message ?? "Failed to retrieve user plan histories.");
            }
            var responseData = result.Data?.Select(dto => new UsersPlanHistoriesResponseViewModel(
                dto.UserId,
                dto.ExternalPlanId,
                dto.AssignedAt,
                dto.EndedAt,
                dto.ResonForChange
            )).ToList();
            return EndPointResponse<List<UsersPlanHistoriesResponseViewModel>>.Success(responseData, "User plan histories retrieved successfully.");
        }
    }
    public record UsersPlanHistoriesResponseViewModel(int UserId, int ExternalPlanId, DateTime AssignedAt, DateTime? EndedAt, string? ReasonForChange);
}

    