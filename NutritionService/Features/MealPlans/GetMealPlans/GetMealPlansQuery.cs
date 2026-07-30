using MediatR;
using NutritionService.Common.Models;

namespace NutritionService.Features.MealPlans.GetMealPlans;

/// <summary>CQRS query for User Story 6.4 — GET /api/v1/nutrition/meal-plans.</summary>
public sealed record GetMealPlansQuery(int Page, int PageSize) : IRequest<PagedResult<MealPlanDto>>;
