namespace SmartCoachService.Features.Chat.GetHistory;

public sealed class ChatMessageDto
{
    public Guid Id { get; init; }
    public string Sender { get; init; } = default!;
    public string Content { get; init; } = default!;
    public List<string> FollowUpSuggestions { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}

public sealed class ChatSessionSummaryDto
{
    public Guid Id { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime LastActivityAt { get; init; }
}

/// <summary>
/// Exactly one of the two lists is populated, matching the two acceptance-criteria branches:
/// Messages when sessionId is supplied, Sessions (paginated) otherwise.
/// </summary>
public sealed class GetChatHistoryResult
{
    public IReadOnlyList<ChatMessageDto>? Messages { get; init; }
    public IReadOnlyList<ChatSessionSummaryDto>? Sessions { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
}
