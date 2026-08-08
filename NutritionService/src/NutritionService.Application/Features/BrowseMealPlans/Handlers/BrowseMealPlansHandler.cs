using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Application.Common.Models;
using NutritionService.Application.Features.BrowseMealPlans.Dtos;
using NutritionService.Application.Features.BrowseMealPlans.Queries;
using NutritionService.Application.Features.BrowseMealPlans.Validators;
using NutritionService.Domain.Common.Interfaces;
using NutritionService.Domain.Entities;

namespace NutritionService.Application.Features.BrowseMealPlans.Handlers;

public sealed class BrowseMealPlansHandler : IRequestHandler<BrowseMealPlansQuery, PagedResult<MealPlanDto>>
{
    private readonly IUnitOfWork _uow;

    public BrowseMealPlansHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PagedResult<MealPlanDto>> Handle(
        BrowseMealPlansQuery request, CancellationToken cancellationToken)
    {
        BrowseMealPlansValidator.EnsureValid(request);

        var repo = _uow.Repository<MealPlan>().Query().AsNoTracking();
        var totalCount = await repo.CountAsync(cancellationToken);

        var plans = await repo
            .OrderBy(p => p.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new MealPlanDto(
                p.Id, p.Name, p.Description,
                p.TargetCalorieRangeMin, p.TargetCalorieRangeMax,
                p.Items
                    .OrderBy(i => i.DayNumber)
                    .Select(i => new MealPlanItemDto(i.DayNumber, i.MealSlot, i.MealId, i.Meal.Name))
                    .ToList()))
            .ToListAsync(cancellationToken);

        return totalCount == 0
            ? PagedResult<MealPlanDto>.Empty(request.Page, request.PageSize)
            : PagedResult<MealPlanDto>.Create(plans, request.Page, request.PageSize, totalCount);
    }
}
