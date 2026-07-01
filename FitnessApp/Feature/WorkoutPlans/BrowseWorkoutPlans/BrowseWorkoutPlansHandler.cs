using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Common;
using WorkoutService.Contracts;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Feature.WorkoutPlans.BrowseWorkoutPlans;

public sealed class BrowseWorkoutPlansHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<BrowseWorkoutPlansQuery, RequestResult<List<BrowseWorkoutPlansResponse>>>
{
    public async Task<RequestResult<List<BrowseWorkoutPlansResponse>>> Handle(
        BrowseWorkoutPlansQuery request,
        CancellationToken cancellationToken)
    {
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

        return RequestResult<List<BrowseWorkoutPlansResponse>>
            .Success(plans);
    }
}