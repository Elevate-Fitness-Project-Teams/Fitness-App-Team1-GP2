using FCEService.Common.Abstract;
using FCEService.Common.RequestResult;
using FCEService.Domain.Aggregates;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCEService.Features.WeightGoalActivity.Commands
{
    public record WeightGoalActivityCommand(Guid userId, PhysicalStats physicalStats, Activity activity, int WorkoutDays) : IRequest<RequestResult<bool>>;

    public class WeightGoalActivityCommandHandler : IRequestHandler<WeightGoalActivityCommand, RequestResult<bool>>
    {
        IGenericRepository<UserFitnessStats> _repository;
        public async Task<RequestResult<bool>> Handle(WeightGoalActivityCommand request, CancellationToken cancellationToken)
        {
            _repository.Add(new UserFitnessStats
            {
                userId = request.userId,
                physicalStats = request.physicalStats,
                activity = request.activity,
                WorkoutDays = request.WorkoutDays
            });
            return RequestResult<bool>.Success(true, "Weight goal activity updated successfully");
        }
    }
}
