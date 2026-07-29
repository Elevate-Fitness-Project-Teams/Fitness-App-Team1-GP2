namespace NutritionService.Features.MealPlans.GetMealPlans;

public sealed class MealPlanItemDto
{
    public Guid MealId { get; init; }
    public string MealName { get; init; } = default!;
    public int DayNumber { get; init; }
    public string MealType { get; init; } = default!;
    public int OrderIndex { get; init; }
}

public sealed class MealPlanDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
    public int TargetCalorieRangeMin { get; init; }
    public int TargetCalorieRangeMax { get; init; }
    public List<MealPlanItemDto> Schedule { get; init; } = new();
}
