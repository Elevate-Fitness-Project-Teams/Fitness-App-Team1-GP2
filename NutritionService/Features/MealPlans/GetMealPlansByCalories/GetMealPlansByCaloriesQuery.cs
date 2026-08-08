using MediatR;

namespace NutritionService.Features.MealPlans.GetMealPlansByCalories;

/// <summary>CQRS query for User Story 6.5 — GET /api/v1/nutrition/meal-plans/by-calories.</summary>
public sealed record GetMealPlansByCaloriesQuery(int? Calories) : IRequest<IReadOnlyList<MealPlanSummaryDto>>;
