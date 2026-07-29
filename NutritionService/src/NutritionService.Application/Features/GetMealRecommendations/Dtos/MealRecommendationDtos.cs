using NutritionService.Application.Common.Models;

namespace NutritionService.Application.Features.GetMealRecommendations.Dtos;

public sealed record MealRecommendationsResponse(
    int UserDailyGoalCalories,
    PagedResult<MealRecommendationDto> RecommendedMeals);

public sealed record MealRecommendationDto(
    Guid Id,
    string Name,
    string Type,
    int Calories,
    decimal ProteinGrams);
