namespace NutritionService.Application.Features.GetMealPlansByCalories.Dtos;

public sealed record MealPlanByCaloriesDto(
    Guid Id,
    string Name,
    int TargetCalorieRangeMin,
    int TargetCalorieRangeMax);
