using FCEService.Common.ViewModels;
using FCEService.Features.UserAssignedPlans.Commands;
using FCEService.Features.UserAssignedPlans.Queries;
using MediatR;

namespace FCEService.Features.UserAssignedPlans.Orchestrators
{
    public record AssignPlanOrchestrator(int userId) : IRequest<RequestResult<bool>>;
    public class AssignPlanOrchestratorHandler : IRequestHandler<AssignPlanOrchestrator, RequestResult<bool>>
    {
        private readonly IMediator _mediator;
        public AssignPlanOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<RequestResult<bool>> Handle(AssignPlanOrchestrator request, CancellationToken cancellationToken)
        {
            var userFitnessStatsResult = await _mediator.Send(new GetUserFitnessStateQuery(request.userId), cancellationToken);
            if (!userFitnessStatsResult.IsSuccess)
            {
                return RequestResult<bool>.Failure(userFitnessStatsResult.Message);
            }
            var userFitnessStats = userFitnessStatsResult.Data;
            var calorieTarget = await _mediator.Send(new GetCalorieTargetByUserId(request.userId), cancellationToken);
            if (!calorieTarget.IsSuccess)
            {
                return RequestResult<bool>.Failure(calorieTarget.Message);
            }
            var userAssignedPlanCommand = await _mediator.Send(new UserAssignedPlanCommand(request.userId, userFitnessStats.goal, calorieTarget.Data, "Default Workout Plan", "Default Nutrition Plan"), cancellationToken);

            return (userAssignedPlanCommand.IsSuccess)
                ? RequestResult<bool>.Success(true, "User assigned plan created successfully.")
                : RequestResult<bool>.Failure(userAssignedPlanCommand.Message);
        }
    }   
}
