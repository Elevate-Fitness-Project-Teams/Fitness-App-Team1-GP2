using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WorkoutService.Common;
using WorkoutService.Contracts;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Feature.WorkoutPlans.BrowseWorkoutPlans;

public sealed class BrowseWorkoutPlansHandler(IUnitOfWork unitOfWork,IMemoryCache memoryCache)
    : IRequestHandler<BrowseWorkoutPlansQuery, RequestResult<List<BrowseWorkoutPlansResponse>>>
{
    private const string CacheKey = "workoutPlans:all";
    public async Task<RequestResult<List<BrowseWorkoutPlansResponse>>> Handle(
        BrowseWorkoutPlansQuery request,
        CancellationToken cancellationToken)
    {
    if(memoryCache.TryGetValue(CacheKey, out List<BrowseWorkoutPlansResponse>? cachedPlans))
        {
            return RequestResult<List<BrowseWorkoutPlansResponse>>.Success(cachedPlans!);
        }

        var plans = await unitOfWork
            .GetRepository<WorkoutPlan>()
            .GetAll()
            .OrderBy(x => x.PlanId)
            .Select(x => new BrowseWorkoutPlansResponse
            {
                PlanId = x.PlanId,
                Name = x.Name,
                Description = x.Description,
                Goal = x.Goal.ToString(),
                Status = x.Status.ToString(),
                Difficulty = x.Difficulty.ToString()
            })
            .ToListAsync(cancellationToken);

        memoryCache.Set(CacheKey, plans, TimeSpan.FromMinutes(10));

        return RequestResult<List<BrowseWorkoutPlansResponse>>
            .Success(plans);
    }
}