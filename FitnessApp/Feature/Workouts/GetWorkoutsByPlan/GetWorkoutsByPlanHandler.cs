using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Common;
using WorkoutService.Contracts;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Feature.Workouts.GetWorkoutsByPlan
{
    public class GetWorkoutsByPlanHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetWorkoutsByPlanQuery, RequestResult<List<GetWorkoutsByPlanResponse>>>
    {
        public async Task<RequestResult<List<GetWorkoutsByPlanResponse>>> Handle(GetWorkoutsByPlanQuery request, CancellationToken cancellationToken)
        {
            var query = unitOfWork.GetRepository<Workout>();
            var existingWorkouts = await query
            .ExistsAsync(w => w.PlanId == request.planId, cancellationToken);

            if(!existingWorkouts)
            {
             return RequestResult<List<GetWorkoutsByPlanResponse>>.Failure(ErrorCode.WorkoutNotFound);
            }

            var workouts = await query
            .GetAll()
            .Where(w => w.PlanId == request.planId)
            .OrderBy(w => w.WorkoutId)
            .Select(x => new GetWorkoutsByPlanResponse
            {
                WorkoutId = x.WorkoutId,
                Name = x.Name,
                Category = x.Category.ToString(),
                Difficulty = x.Difficulty.ToString(),
                DurationInMinutes = x.DurationInMinutes,
                PlanId = x.PlanId,
                PlanName = x.WorkoutPlan!.Name
            }).ToListAsync(cancellationToken);

            return RequestResult<List<GetWorkoutsByPlanResponse>>.Success(workouts);
        }
    }
    }
