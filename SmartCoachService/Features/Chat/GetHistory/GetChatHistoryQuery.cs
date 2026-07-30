using MediatR;

namespace SmartCoachService.Features.Chat.GetHistory;

/// <summary>CQRS query for User Story 7.2 — GET /api/v1/smart-coach/history.</summary>
public sealed record GetChatHistoryQuery(Guid? SessionId, int Page, int PageSize) : IRequest<GetChatHistoryResult>;
