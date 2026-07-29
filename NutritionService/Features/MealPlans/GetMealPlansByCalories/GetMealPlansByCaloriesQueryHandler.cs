using MediatR;
using NutritionService.Common.Abstractions;

namespace NutritionService.Features.MealPlans.GetMealPlansByCalories;

public sealed class GetMealPlansByCaloriesQueryHandler
    : IRequestHandler<GetMealPlansByCaloriesQuery, IReadOnlyList<MealPlanSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMealPlansByCaloriesQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<MealPlanSummaryDto>> Handle(GetMealPlansByCaloriesQuery request, CancellationToken cancellationToken)
    {
        var calories = request.Calories!.Value;

        var plans = await _unitOfWork.MealPlans.FindAsync(
            p => p.TargetCalorieRangeMin <= calories && p.TargetCalorieRangeMax >= calories,
            cancellationToken);

        return plans.Select(p => new MealPlanSummaryDto
        {
            Id = p.Id,
            Name = p.Name,
            TargetCalorieRangeMin = p.TargetCalorieRangeMin,
            TargetCalorieRangeMax = p.TargetCalorieRangeMax
        }).ToList();
    }
}
