using MediatR;
using WorkoutService.Common;
using WorkoutService.Features.Workouts.BrowseWorkouts;

namespace WorkoutService.Feature.Workouts.BrowseWorkouts
{
    public sealed record BrowseWorkoutsQuery(
    int page = 1
    , int pageSize = 10
    , string? category = null
    , string? difficulty = null
    , int duration = 0
    , string? search = null) : IRequest<PagedResult<BrowseWorkoutsResponse>>;
} 
