using FCEService.Common.Abstract;
using FCEService.Common.ViewModels;
using FCEService.Domain.Entities;
using FCEService.Domain.Enums;
using MediatR;

namespace FCEService.Features.UserAssignedPlans.Commands
{
    public record UserAssignedPlanCommand(int userId, Goal userGoal, double IntakeClaorie, string WorkoutPlanName, string NutritionPlanName) : IRequest<RequestResult<bool>>;
    public class UserAssignedPlanCommandHandler : IRequestHandler<UserAssignedPlanCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<UserAssignedPlan> _repository;
        public UserAssignedPlanCommandHandler(IGenericRepository<UserAssignedPlan> repository)
        {
            _repository = repository;
        }
        public async Task<RequestResult<bool>> Handle(UserAssignedPlanCommand request, CancellationToken cancellationToken)
        {
            var userAssignedPlan = UserAssignedPlan.Create(request.userId, request.userGoal, request.IntakeClaorie, request.WorkoutPlanName, request.NutritionPlanName);
            _repository.Add(userAssignedPlan);
            await _repository.SaveChangeAsync(cancellationToken);
            return RequestResult<bool>.Success(true, "User assigned plan created successfully.");
        }
    }
}
