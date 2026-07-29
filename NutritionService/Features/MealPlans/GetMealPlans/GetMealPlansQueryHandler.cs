using MediatR;
using NutritionService.Common.Abstractions;
using NutritionService.Common.Models;

namespace NutritionService.Features.MealPlans.GetMealPlans;

public sealed class GetMealPlansQueryHandler : IRequestHandler<GetMealPlansQuery, PagedResult<MealPlanDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMealPlansQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<PagedResult<MealPlanDto>> Handle(GetMealPlansQuery request, CancellationToken cancellationToken)
    {
        var baseQuery = _unitOfWork.MealPlans.Query();
        var totalCount = baseQuery.Count();

        var plans = baseQuery
            .OrderBy(p => p.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = new List<MealPlanDto>();
        foreach (var plan in plans)
        {
            var items = (await _unitOfWork.MealPlanItems.FindAsync(i => i.MealPlanId == plan.Id, cancellationToken))
                .OrderBy(i => i.DayNumber).ThenBy(i => i.OrderIndex)
                .ToList();

            var mealNames = new Dictionary<Guid, string>();
            foreach (var item in items)
            {
                if (!mealNames.ContainsKey(item.MealId))
                {
                    var meal = await _unitOfWork.Meals.GetByIdAsync(item.MealId, cancellationToken);
                    mealNames[item.MealId] = meal?.Name ?? "Unknown";
                }
            }

            dtos.Add(new MealPlanDto
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                TargetCalorieRangeMin = plan.TargetCalorieRangeMin,
                TargetCalorieRangeMax = plan.TargetCalorieRangeMax,
                Schedule = items.Select(i => new MealPlanItemDto
                {
                    MealId = i.MealId,
                    MealName = mealNames[i.MealId],
                    DayNumber = i.DayNumber,
                    MealType = i.MealType.ToString(),
                    OrderIndex = i.OrderIndex
                }).ToList()
            });
        }

        return new PagedResult<MealPlanDto>
        {
            Items = dtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
