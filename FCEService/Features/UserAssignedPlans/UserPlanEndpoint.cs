using FCEService.Common.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCEService.Features.UserAssignedPlans
{
    [ApiController]
    [Route("api/v1/user-assigned-plans")]
    public class UserPlanEndpoint : ControllerBase
    {
        [HttpPost("{userId}")]
        public async Task<EndPointResponse<bool>> AssignPlan(int userId, [FromServices] IMediator mediator)
        {
            var result = await mediator.Send(new Orchestrators.AssignPlanOrchestrator(userId));
            if (!result.IsSuccess)
            {
                return EndPointResponse<bool>.Failure(result.Message, ResponseErrorCode.InternalServerError);
            }
            return EndPointResponse<bool>.Success(result.Data);
        }
    }
}
