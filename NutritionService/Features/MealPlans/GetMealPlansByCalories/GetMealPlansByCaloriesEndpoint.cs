using MediatR;
using Microsoft.AspNetCore.Mvc;
using NutritionService.Common.Abstractions;
using NutritionService.Common.Models;

namespace NutritionService.Features.MealPlans.GetMealPlansByCalories;

/// <summary>GET /api/v1/nutrition/meal-plans/by-calories — User Story 6.5.</summary>
public sealed class GetMealPlansByCaloriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/nutrition/meal-plans/by-calories", HandleAsync)
            .RequireAuthorization()
            .WithTags("Nutrition")
            .WithName("GetMealPlansByCalories")
            .Produces<ApiResponse<IReadOnlyList<MealPlanSummaryDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync([FromServices] ISender sender, [FromQuery] int? calories, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMealPlansByCaloriesQuery(calories), cancellationToken);
        return Results.Ok(ApiResponse<IReadOnlyList<MealPlanSummaryDto>>.Ok(result));
    }
}
