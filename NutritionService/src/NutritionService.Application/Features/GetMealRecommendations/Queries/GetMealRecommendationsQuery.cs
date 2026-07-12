using MediatR;
using NutritionService.Application.Features.GetMealRecommendations.Dtos;

namespace NutritionService.Application.Features.GetMealRecommendations.Queries;

public sealed record GetMealRecommendationsQuery(
    string? MealType,
    int Page,
    int PageSize,
    int? MaxCalories,
    decimal? MinProtein) : IRequest<MealRecommendationsResponse>;
