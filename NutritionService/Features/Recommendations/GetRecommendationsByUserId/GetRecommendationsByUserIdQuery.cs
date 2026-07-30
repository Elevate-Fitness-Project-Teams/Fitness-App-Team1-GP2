using MediatR;
using NutritionService.Features.Recommendations.GetRecommendations;

namespace NutritionService.Features.Recommendations.GetRecommendationsByUserId;

/// <summary>CQRS query for User Story 6.2 — GET /api/v1/nutrition/recommendations/{userId}.</summary>
public sealed record GetRecommendationsByUserIdQuery(Guid UserId, int Page, int PageSize)
    : IRequest<GetRecommendationsResult>;
