using FCEService.Common.ViewModels;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;
using FCEService.Features.WeightGoalActivity.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCEService.Features.WeightGoalActivity
{
    [ApiController]
    [Route("api/v1/weight-goal-activity")]
    public class WeightGoalActivityEndpoint : ControllerBase
    {
        private readonly IMediator _mediator;
        public WeightGoalActivityEndpoint(IMediator mediator) { 
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<EndPointResponse<int>> CreateWeightGoalActivity([FromBody] WeightGoalActivityRequestViewModel command)
        {
            var result = await _mediator.Send(new WeightGoalActivityCommand(
                userId: command.userId,
                physicalStats: command.physicalStats,
                activity: command.activity,
                WorkoutDays: command.WorkoutDays,
                IsActive: command.IsActive
            ));

            if (!result.IsSuccess)
            {
                return EndPointResponse<int>.Failure(result.Message, ResponseErrorCode.InternalServerError);
            }

            return EndPointResponse<int>.Success(result.Data);
        }
    }

    public record WeightGoalActivityRequestViewModel(int userId, PhysicalStats physicalStats, Activity activity, int WorkoutDays, bool IsActive);
    
}
