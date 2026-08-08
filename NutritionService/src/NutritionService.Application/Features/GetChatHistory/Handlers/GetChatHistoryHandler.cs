using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Application.Common.Exceptions;
using NutritionService.Application.Common.Models;
using NutritionService.Domain.Common.Interfaces;
using SmartCoachService.Application.Common.Interfaces;
using SmartCoachService.Application.Features.GetChatHistory.Dtos;
using SmartCoachService.Application.Features.GetChatHistory.Queries;
using SmartCoachService.Domain.Entities;

namespace SmartCoachService.Application.Features.GetChatHistory.Handlers;

public sealed class GetChatHistoryHandler : IRequestHandler<GetChatHistoryQuery, ChatHistoryResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetChatHistoryHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<ChatHistoryResponse> Handle(
        GetChatHistoryQuery request, CancellationToken cancellationToken)
    {
        if (request.Page < 1)
            throw new ValidationAppException("VAL_PAGE_INVALID", "page must be >= 1.");
        if (request.PageSize is < 1 or > 100)
            throw new ValidationAppException("VAL_PAGE_SIZE_INVALID", "pageSize must be between 1 and 100.");

        var userId = _currentUser.UserId;

        // ── Branch A: sessionId supplied ──────────────────────────────────────────
        if (request.SessionId.HasValue)
        {
            // Ownership check: session must belong to the caller
            var sessionExists = await _uow.Repository<ChatSession>()
                .Query()
                .AsNoTracking()
                .AnyAsync(s => s.Id == request.SessionId.Value && s.UserId == userId, cancellationToken);

            if (!sessionExists)
                throw new SessionNotFoundException(request.SessionId.Value);

            var messages = await _uow.Repository<ChatMessage>()
                .Query()
                .AsNoTracking()
                .Where(m => m.SessionId == request.SessionId.Value)
                .OrderBy(m => m.SentAtUtc)
                .Select(m => new ChatMessageDto(m.Id, m.Sender.ToString(), m.Content, m.SentAtUtc))
                .ToListAsync(cancellationToken);

            return new ChatHistoryResponse(Messages: messages, Sessions: null);
        }

        // ── Branch B: no sessionId → paginated session list ───────────────────────
        var sessionQuery = _uow.Repository<ChatSession>()
            .Query()
            .AsNoTracking()
            .Where(s => s.UserId == userId);

        var total = await sessionQuery.CountAsync(cancellationToken);

        var sessions = await sessionQuery
            .OrderByDescending(s => s.LastActivityAtUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new ChatSessionSummaryDto(
                s.Id,
                s.CreatedAtUtc,
                s.LastActivityAtUtc,
                s.Messages.Count))
            .ToListAsync(cancellationToken);

        var paged = total == 0
            ? PagedResult<ChatSessionSummaryDto>.Empty(request.Page, request.PageSize)
            : PagedResult<ChatSessionSummaryDto>.Create(sessions, request.Page, request.PageSize, total);

        return new ChatHistoryResponse(Messages: null, Sessions: paged);
    }
}
