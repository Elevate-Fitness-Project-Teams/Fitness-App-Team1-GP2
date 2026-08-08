using MediatR;
using SmartCoachService.Common.Abstractions;
using SmartCoachService.Common.Exceptions;

namespace SmartCoachService.Features.Chat.GetHistory;

public sealed class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, GetChatHistoryResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public GetChatHistoryQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<GetChatHistoryResult> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
    {
        if (request.SessionId.HasValue)
        {
            var session = await _unitOfWork.ChatSessions.GetByIdAsync(request.SessionId.Value, cancellationToken);
            if (session is null || session.UserId != _currentUser.UserId)
                throw new NotFoundException("RES_SESSION_NOT_FOUND", $"Chat session {request.SessionId} was not found.");

            var messages = (await _unitOfWork.ChatMessages.FindAsync(m => m.SessionId == session.Id, cancellationToken))
                .OrderBy(m => m.CreatedAt)
                .Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    Sender = m.Sender.ToString(),
                    Content = m.Content,
                    FollowUpSuggestions = m.FollowUpSuggestions,
                    CreatedAt = m.CreatedAt
                })
                .ToList();

            return new GetChatHistoryResult
            {
                Messages = messages,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = messages.Count
            };
        }

        var sessionsQuery = _unitOfWork.ChatSessions.Query().Where(s => s.UserId == _currentUser.UserId);
        var totalCount = sessionsQuery.Count();

        var sessions = sessionsQuery
            .OrderByDescending(s => s.LastActivityAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new ChatSessionSummaryDto { Id = s.Id, StartedAt = s.StartedAt, LastActivityAt = s.LastActivityAt })
            .ToList();

        return new GetChatHistoryResult
        {
            Sessions = sessions,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
