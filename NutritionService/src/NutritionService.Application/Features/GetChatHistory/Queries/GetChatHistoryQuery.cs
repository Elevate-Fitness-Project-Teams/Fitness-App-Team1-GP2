using MediatR;
using SmartCoachService.Application.Features.GetChatHistory.Dtos;

namespace SmartCoachService.Application.Features.GetChatHistory.Queries;

public sealed record GetChatHistoryQuery(
    Guid? SessionId,
    int Page,
    int PageSize) : IRequest<ChatHistoryResponse>;
