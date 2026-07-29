namespace NutritionService.Features.MealPlans.GetMealPlansByCalories;

public sealed class MealPlanSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public int TargetCalorieRangeMin { get; init; }
    public int TargetCalorieRangeMax { get; init; }
}
