namespace NutritionService.Application.Features.BrowseMealPlans.Dtos;

public sealed record MealPlanDto(
    Guid Id,
    string Name,
    string Description,
    int TargetCalorieRangeMin,
    int TargetCalorieRangeMax,
    IReadOnlyList<MealPlanItemDto> Schedule);

public sealed record MealPlanItemDto(int DayNumber, string MealSlot, Guid MealId, string MealName);
