using MediatR;
using Microsoft.AspNetCore.Mvc;
using NutritionService.Common.Abstractions;
using NutritionService.Common.Models;
using NutritionService.Features.Recommendations.GetRecommendations;

namespace NutritionService.Features.Recommendations.GetRecommendationsByUserId;

/// <summary>GET /api/v1/nutrition/recommendations/{userId} — User Story 6.2.</summary>
public sealed class GetRecommendationsByUserIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/nutrition/recommendations/{userId:guid}", HandleAsync)
            .RequireAuthorization()
            .WithTags("Nutrition")
            .WithName("GetRecommendationsByUserId")
            .Produces<ApiResponse<GetRecommendationsResult>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ISender sender,
        Guid userId,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetRecommendationsByUserIdQuery(userId, page <= 0 ? 1 : page, pageSize <= 0 ? 20 : pageSize),
            cancellationToken);

        return Results.Ok(ApiResponse<GetRecommendationsResult>.Ok(result));
    }
}
