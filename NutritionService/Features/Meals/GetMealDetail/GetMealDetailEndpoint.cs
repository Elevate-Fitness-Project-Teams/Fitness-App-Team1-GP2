using MediatR;
using Microsoft.AspNetCore.Mvc;
using NutritionService.Common.Abstractions;
using NutritionService.Common.Models;

namespace NutritionService.Features.Meals.GetMealDetail;

/// <summary>GET /api/v1/nutrition/meals/{id} — User Story 6.3.</summary>
public sealed class GetMealDetailEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/nutrition/meals/{id:guid}", HandleAsync)
            .RequireAuthorization()
            .WithTags("Nutrition")
            .WithName("GetMealDetail")
            .Produces<ApiResponse<MealDetailDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync([FromServices] ISender sender, Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMealDetailQuery(id), cancellationToken);
        return Results.Ok(ApiResponse<MealDetailDto>.Ok(result));
    }
}
