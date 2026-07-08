using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WorkoutService.Common;
using WorkoutService.Contracts;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Feature.WorkoutPlans.GetWorkoutPlanById;

public sealed class GetWorkoutPlanByIdHandler(IUnitOfWork unitOfWork,IMemoryCache memoryCache)
    : IRequestHandler<GetWorkoutPlanByIdQuery, RequestResult<GetWorkoutPlanByIdResponse>>
{
    public async Task<RequestResult<GetWorkoutPlanByIdResponse>> Handle(
        GetWorkoutPlanByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"workout-plan:{request.PlanId}";
        
        if (memoryCache.TryGetValue(cacheKey, out GetWorkoutPlanByIdResponse? cachedPlan))
        {
            return RequestResult<GetWorkoutPlanByIdResponse>.Success(cachedPlan!);
        }

        var plan = await unitOfWork
            .GetRepository<WorkoutPlan>()
            .GetAll()
            .Where(x => x.PlanId == request.PlanId)
            .Select(x => new GetWorkoutPlanByIdResponse
            {
                PlanId = x.PlanId,
                Name = x.Name,
                Description = x.Description,
                Goal = x.Goal.ToString(),
                Status = x.Status.ToString(),
                Difficulty = x.Difficulty.ToString()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (plan is null)
        {
            return RequestResult<GetWorkoutPlanByIdResponse>
                .Failure(ErrorCode.WorkoutPlanNotFound);
        }

        memoryCache.Set(cacheKey, plan, TimeSpan.FromMinutes(10));

        return RequestResult<GetWorkoutPlanByIdResponse>
            .Success(plan);
    }
}