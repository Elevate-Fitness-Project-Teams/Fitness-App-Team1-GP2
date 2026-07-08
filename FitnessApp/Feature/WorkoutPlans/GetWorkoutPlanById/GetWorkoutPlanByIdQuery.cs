using MediatR;
using WorkoutService.Common;

namespace WorkoutService.Feature.WorkoutPlans.GetWorkoutPlanById;

public sealed record GetWorkoutPlanByIdQuery(string PlanId)
    : IRequest<RequestResult<GetWorkoutPlanByIdResponse>>;