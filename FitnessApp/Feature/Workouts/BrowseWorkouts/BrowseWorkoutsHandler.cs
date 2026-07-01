using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Common;
using WorkoutService.Contracts;
using WorkoutService.Domain.Entities;
using WorkoutService.Features.Workouts.BrowseWorkouts;

namespace WorkoutService.Feature.Workouts.BrowseWorkouts;

public class BrowseWorkoutsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<BrowseWorkoutsQuery, RequestResult<PagedResult<BrowseWorkoutsResponse>>>
{
    public async Task<RequestResult<PagedResult<BrowseWorkoutsResponse>>> Handle(
        BrowseWorkoutsQuery request,
        CancellationToken cancellationToken)
    {
        var query = unitOfWork
            .GetRepository<Workout>()
            .GetAll();

        if (!string.IsNullOrWhiteSpace(request.category))
        {
            query = query.Where(x => x.Category.ToString() == request.category);
        }

        if (!string.IsNullOrWhiteSpace(request.difficulty))
        {
            query = query.Where(x => x.Difficulty.ToString() == request.difficulty);
        }

        if (!string.IsNullOrWhiteSpace(request.search))
        {
            query = query.Where(x => x.Name.Contains(request.search));
        }

        if (request.duration > 0)
        {
            query = query.Where(x => x.DurationInMinutes == request.duration);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.WorkoutId)
            .Skip((request.page - 1) * request.pageSize)
            .Take(request.pageSize)
            .Select(x => new BrowseWorkoutsResponse
            {
                WorkoutId = x.WorkoutId,
                Category = x.Category.ToString(),
                Difficulty = x.Difficulty.ToString(),
                DurationInMinutes = x.DurationInMinutes,
                Name = x.Name,
                PlanId = x.PlanId,
                PlanName = x.WorkoutPlan!.Name
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<BrowseWorkoutsResponse>
        {
            Items = items,
            Page = request.page,
            PageSize = request.pageSize,
            TotalCount = totalCount
        };

        return RequestResult<PagedResult<BrowseWorkoutsResponse>>
            .Success(pagedResult);
    }
}