using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCoachService.Application.Features.GetChatHistory.Queries;
using SmartCoachService.Application.Features.GetHomeFeed.Queries;
using SmartCoachService.Application.Features.SendChatMessage.Commands;

namespace SmartCoachService.Api.Controllers;

[ApiController]
[Authorize]
public class SmartCoachController : ControllerBase
{
    private readonly IMediator _mediator;

    public SmartCoachController(IMediator mediator) => _mediator = mediator;

    // Story 7.1 — POST /api/v1/smart-coach/chat
    [HttpPost("api/v1/smart-coach/chat")]
    public async Task<IActionResult> SendChatMessage(
        [FromBody] SendChatMessageRequest body,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new SendChatMessageCommand(body.Message, body.SessionId), ct);
        return Ok(result);
    }

    // Story 7.2 — GET /api/v1/smart-coach/history
    [HttpGet("api/v1/smart-coach/history")]
    public async Task<IActionResult> GetChatHistory(
        [FromQuery] Guid? sessionId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetChatHistoryQuery(sessionId, page, pageSize), ct);

        // Return whichever branch was populated
        return Ok(result.Messages is not null
            ? (object)result.Messages
            : result.Sessions!);
    }

    // Story 7.3 — GET /api/v1/home
    [HttpGet("api/v1/home")]
    public async Task<IActionResult> GetHomeFeed(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetHomeFeedQuery(), ct);
        return Ok(result);
    }
}

// Inline request body model (no separate file needed for a simple 2-field record)
public sealed record SendChatMessageRequest(string Message, Guid? SessionId);
