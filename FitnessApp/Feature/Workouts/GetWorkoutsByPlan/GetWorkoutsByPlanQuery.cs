using MediatR;
using WorkoutService.Common;

namespace WorkoutService.Feature.Workouts.GetWorkoutsByPlan
{
    public sealed record GetWorkoutsByPlanQuery(string planId) : IRequest<RequestResult<List<GetWorkoutsByPlanResponse>>>;
    

}
