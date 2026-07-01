using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Common;
using WorkoutService.Contracts;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Feature.Exercises.BrowseExercises;

public sealed class BrowseExercisesHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<BrowseExercisesQuery, RequestResult<List<BrowseExercisesResponse>>>
{
    public async Task<RequestResult<List<BrowseExercisesResponse>>> Handle(
        BrowseExercisesQuery request,
        CancellationToken cancellationToken)
    {
        var exercises = await unitOfWork
            .GetRepository<Exercise>()
            .GetAll()
            .OrderBy(x => x.ExerciseId)
            .Select(x => new BrowseExercisesResponse
            {
                ExerciseId = x.ExerciseId,
                Name = x.Name,
                TargetMuscles = x.TargetMuscles,
                Equipment = x.Equipment,
                Difficulty = x.Difficulty.ToString(),
                Description = x.Description,
                VideoUrl = x.VideoUrl
            })
            .ToListAsync(cancellationToken);

        return RequestResult<List<BrowseExercisesResponse>>
            .Success(exercises);
    }
}