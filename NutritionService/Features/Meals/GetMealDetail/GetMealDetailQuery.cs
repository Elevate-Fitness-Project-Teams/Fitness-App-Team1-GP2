using MediatR;

namespace NutritionService.Features.Meals.GetMealDetail;

/// <summary>CQRS query for User Story 6.3 — GET /api/v1/nutrition/meals/{id}.</summary>
public sealed record GetMealDetailQuery(Guid Id) : IRequest<MealDetailDto>;
