using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCoachService.Common.Abstractions;
using SmartCoachService.Common.Models;

namespace SmartCoachService.Features.Chat.GetHistory;

/// <summary>GET /api/v1/smart-coach/history — User Story 7.2.</summary>
public sealed class GetChatHistoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/smart-coach/history", HandleAsync)
            .RequireAuthorization()
            .WithTags("SmartCoach")
            .WithName("GetChatHistory")
            .Produces<ApiResponse<GetChatHistoryResult>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ISender sender,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] Guid? sessionId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetChatHistoryQuery(sessionId, page <= 0 ? 1 : page, pageSize <= 0 ? 20 : pageSize), cancellationToken);
        return Results.Ok(ApiResponse<GetChatHistoryResult>.Ok(result));
    }
}
