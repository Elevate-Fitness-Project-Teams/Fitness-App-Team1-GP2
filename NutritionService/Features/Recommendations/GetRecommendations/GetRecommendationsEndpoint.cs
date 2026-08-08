using MediatR;
using Microsoft.AspNetCore.Mvc;
using NutritionService.Common.Abstractions;
using NutritionService.Common.Models;
using NutritionService.Domain.Enums;

namespace NutritionService.Features.Recommendations.GetRecommendations;

/// <summary>GET /api/v1/nutrition/recommendations — User Story 6.1.</summary>
public sealed class GetRecommendationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/nutrition/recommendations", HandleAsync)
            .RequireAuthorization()
            .WithTags("Nutrition")
            .WithName("GetPersonalizedRecommendations")
            .Produces<ApiResponse<GetRecommendationsResult>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ISender sender,
        [FromQuery] MealType? mealType,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] int? maxCalories,
        [FromQuery] decimal? minProtein,
        CancellationToken cancellationToken)
    {
        var query = new GetRecommendationsQuery(
            mealType,
            page <= 0 ? 1 : page,
            pageSize <= 0 ? 20 : pageSize,
            maxCalories,
            minProtein);

        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(ApiResponse<GetRecommendationsResult>.Ok(result));
    }
}
