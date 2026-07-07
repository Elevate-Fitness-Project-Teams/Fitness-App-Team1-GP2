using FCEService.Common.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FCEService.Features.GetUserPlanHistoryByUserId
{
    [Route("api/v1/fitness/plans/user/")]
    [ApiController]
    public class GetUserPlanHistoryByEndpoint : ControllerBase
    {
        private readonly IMediator mediator;

        public GetUserPlanHistoryByEndpoint(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpGet("{userId}")]
        public async Task<EndPointResponse<List<UserPlanHistoryResponseViewModel>>> GetUserPlanHistory(int userId)
        {
            var result = await mediator.Send(new Queries.GetUserPlanHistoryByIdQuery(userId));
            if(!result.IsSuccess)
            {
                return EndPointResponse<List<UserPlanHistoryResponseViewModel>>.Failure(result.Message ?? "Failed to retrieve user plan history.");
            }
            var responseData = result.Data?.Select(dto => new UserPlanHistoryResponseViewModel(
                dto.UserId,
                dto.ExternalPlanId,
                dto.AssignedAt,
                dto.EndedAt,
                dto.ResonForChange
            )).ToList();
            return EndPointResponse<List<UserPlanHistoryResponseViewModel>>.Success(responseData, "User plan history retrieved successfully.");
        }
    }
    public record UserPlanHistoryResponseViewModel(int UserId, int ExternalPlanId, DateTime AssignedAt, DateTime? EndedAt, string? ReasonForChange);

}
