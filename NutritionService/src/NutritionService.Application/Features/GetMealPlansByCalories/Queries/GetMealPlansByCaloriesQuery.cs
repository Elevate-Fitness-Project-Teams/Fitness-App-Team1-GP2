using MediatR;
using NutritionService.Application.Features.GetMealPlansByCalories.Dtos;

namespace NutritionService.Application.Features.GetMealPlansByCalories.Queries;

// `Calories` is nullable on purpose to model the "missing" branch explicitly —
// see Validators/GetMealPlansByCaloriesValidator.cs for the resolved gap.
public sealed record GetMealPlansByCaloriesQuery(int? Calories) : IRequest<IReadOnlyList<MealPlanByCaloriesDto>>;
