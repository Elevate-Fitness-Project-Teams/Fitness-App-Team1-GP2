using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCoachService.Common.Abstractions;
using SmartCoachService.Common.Models;

namespace SmartCoachService.Features.HomeFeed.GetHomeFeed;

/// <summary>GET /api/v1/home — User Story 7.3.</summary>
public sealed class GetHomeFeedEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/home", HandleAsync)
            .RequireAuthorization()
            .WithTags("HomeFeed")
            .WithName("GetHomeFeed")
            .Produces<ApiResponse<HomeFeedDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status503ServiceUnavailable);
    }

    private static async Task<IResult> HandleAsync([FromServices] ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetHomeFeedQuery(), cancellationToken);
        return Results.Ok(ApiResponse<HomeFeedDto>.Ok(result));
    }
}
