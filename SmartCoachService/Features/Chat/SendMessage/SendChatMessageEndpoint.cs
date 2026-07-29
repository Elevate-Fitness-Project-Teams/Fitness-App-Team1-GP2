using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCoachService.Common.Abstractions;
using SmartCoachService.Common.Models;

namespace SmartCoachService.Features.Chat.SendMessage;

/// <summary>POST /api/v1/smart-coach/chat — User Story 7.1.</summary>
public sealed class SendChatMessageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/smart-coach/chat", HandleAsync)
            .RequireAuthorization()
            .WithTags("SmartCoach")
            .WithName("SendChatMessage")
            .Produces<ApiResponse<SendChatMessageResult>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<object>>(StatusCodes.Status403Forbidden)
            .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ISender sender,
        [FromBody] SendChatMessageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SendChatMessageCommand(request.Message, request.SessionId), cancellationToken);
        return Results.Ok(ApiResponse<SendChatMessageResult>.Ok(result));
    }
}
