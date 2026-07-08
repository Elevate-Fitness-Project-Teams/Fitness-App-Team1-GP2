using MediatR;
using WorkoutService.Common;

namespace WorkoutService.Feature.Exercises.BrowseExercises;

public sealed record BrowseExercisesQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<RequestResult<PagedResult<BrowseExercisesResponse>>>;