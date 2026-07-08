using MediatR;
using WorkoutService.Common;

namespace WorkoutService.Feature.WorkoutPlans.BrowseWorkoutPlans;

public sealed record BrowseWorkoutPlansQuery
    : IRequest<RequestResult<List<BrowseWorkoutPlansResponse>>>;