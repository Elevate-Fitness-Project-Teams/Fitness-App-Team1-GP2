using MediatR;
using NutritionService.Domain.Enums;

namespace NutritionService.Features.Recommendations.GetRecommendations;

/// <summary>CQRS query for User Story 6.1 — GET /api/v1/nutrition/recommendations.</summary>
public sealed record GetRecommendationsQuery(
    MealType? MealType,
    int Page,
    int PageSize,
    int? MaxCalories,
    decimal? MinProtein) : IRequest<GetRecommendationsResult>;
