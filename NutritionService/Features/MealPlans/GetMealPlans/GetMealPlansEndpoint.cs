using MediatR;
using Microsoft.AspNetCore.Mvc;
using NutritionService.Common.Abstractions;
using NutritionService.Common.Models;

namespace NutritionService.Features.MealPlans.GetMealPlans;

/// <summary>GET /api/v1/nutrition/meal-plans — User Story 6.4.</summary>
public sealed class GetMealPlansEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/nutrition/meal-plans", HandleAsync)
            .RequireAuthorization()
            .WithTags("Nutrition")
            .WithName("BrowseMealPlans")
            .Produces<ApiResponse<PagedResult<MealPlanDto>>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ISender sender,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMealPlansQuery(page <= 0 ? 1 : page, pageSize <= 0 ? 20 : pageSize), cancellationToken);
        return Results.Ok(ApiResponse<PagedResult<MealPlanDto>>.Ok(result));
    }
}
