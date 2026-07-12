using NutritionService.Application.Common.Models;

namespace SmartCoachService.Application.Features.GetChatHistory.Dtos;

public sealed record ChatMessageDto(
    Guid Id,
    string Sender,
    string Content,
    DateTime SentAtUtc);

public sealed record ChatSessionSummaryDto(
    Guid Id,
    DateTime CreatedAtUtc,
    DateTime LastActivityAtUtc,
    int MessageCount);

public sealed record ChatHistoryResponse(
    IReadOnlyList<ChatMessageDto>? Messages,

    PagedResult<ChatSessionSummaryDto>? Sessions);
