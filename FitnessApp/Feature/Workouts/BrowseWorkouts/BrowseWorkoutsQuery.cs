using MediatR;
using WorkoutService.Common;
using WorkoutService.Features.Workouts.BrowseWorkouts;

namespace WorkoutService.Feature.Workouts.BrowseWorkouts;

public sealed record BrowseWorkoutsQuery(
    int page = 1,
    int pageSize = 20,
    string? category = null,
    string? difficulty = null,
    int? duration = null,
    string? search = null
) : IRequest<RequestResult<PagedResult<BrowseWorkoutsResponse>>>;