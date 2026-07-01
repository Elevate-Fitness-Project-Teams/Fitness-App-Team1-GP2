using MediatR;
using WorkoutService.Common;

namespace WorkoutService.Feature.Exercises.BrowseExercises;

public sealed record BrowseExercisesQuery
    : IRequest<RequestResult<List<BrowseExercisesResponse>>>;