using MediatR;
using NutritionService.Application.Features.GetMealRecommendations.Dtos;

namespace NutritionService.Application.Features.GetMealRecommendationsByUserId.Queries;

public sealed record GetMealRecommendationsByUserIdQuery(
    Guid UserId,
    string? MealType,
    int Page,
    int PageSize,
    int? MaxCalories,
    decimal? MinProtein) : IRequest<MealRecommendationsResponse>;
