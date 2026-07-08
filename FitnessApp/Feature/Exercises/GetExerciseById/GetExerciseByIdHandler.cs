using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WorkoutService.Common;
using WorkoutService.Contracts;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Feature.Exercises.GetExerciseById;

public sealed class GetExerciseByIdHandler(IUnitOfWork unitOfWork,IMemoryCache memoryCache)
    : IRequestHandler<GetExerciseByIdQuery, RequestResult<GetExerciseByIdResponse>>
{
    public async Task<RequestResult<GetExerciseByIdResponse>> Handle(
        GetExerciseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"exercise:{request.Id}";

        if(memoryCache.TryGetValue(cacheKey, out GetExerciseByIdResponse? cachedExercise))
        {
            return RequestResult<GetExerciseByIdResponse>.Success(cachedExercise!);
        }
    
    var exercise = await unitOfWork
            .GetRepository<Exercise>()
            .GetAll()
            .Where(x => x.ExerciseId == request.Id)
            .Select(x => new GetExerciseByIdResponse
            {
                ExerciseId = x.ExerciseId,
                Name = x.Name,
                TargetMuscles = x.TargetMuscles,
                Equipment = x.Equipment,
                Difficulty = x.Difficulty.ToString(),
                Description = x.Description,
                VideoUrl = x.VideoUrl
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (exercise is null)
        {
            return RequestResult<GetExerciseByIdResponse>
                .Failure(ErrorCode.ExerciseNotFound);
        }
        memoryCache.Set(cacheKey, exercise, TimeSpan.FromMinutes(10));
        return RequestResult<GetExerciseByIdResponse>
            .Success(exercise);
    }
}