namespace SmartCoachService.Features.Chat.SendMessage;

public sealed class SendChatMessageResult
{
    public Guid SessionId { get; init; }
    public string Reply { get; init; } = default!;
    public List<string> FollowUpSuggestions { get; init; } = new();
}
