using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WorkoutService.Common;
using WorkoutService.Contracts;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Feature.Exercises.BrowseExercises;

public sealed class BrowseExercisesHandler(
    IUnitOfWork unitOfWork,
    IMemoryCache memoryCache)
    : IRequestHandler<BrowseExercisesQuery, RequestResult<PagedResult<BrowseExercisesResponse>>>
{
    public async Task<RequestResult<PagedResult<BrowseExercisesResponse>>> Handle(
        BrowseExercisesQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"exercises:page={request.Page}:pageSize={request.PageSize}";

        if (memoryCache.TryGetValue(cacheKey, out PagedResult<BrowseExercisesResponse>? cachedExercises))
        {
            return RequestResult<PagedResult<BrowseExercisesResponse>>
                .Success(cachedExercises!);
        }

        var query = unitOfWork
            .GetRepository<Exercise>()
            .GetAll();

        var totalCount = await query.CountAsync(cancellationToken);

        var exercises = await query
            .OrderBy(x => x.ExerciseId)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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

        var pagedResult = new PagedResult<BrowseExercisesResponse>
        {
            Items = exercises,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        memoryCache.Set(cacheKey, pagedResult, TimeSpan.FromMinutes(10));

        return RequestResult<PagedResult<BrowseExercisesResponse>>
            .Success(pagedResult);
    }
}