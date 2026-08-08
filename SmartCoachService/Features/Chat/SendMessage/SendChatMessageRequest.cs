namespace SmartCoachService.Features.Chat.SendMessage;

public sealed class SendChatMessageRequest
{
    public string Message { get; init; } = default!;
    public Guid? SessionId { get; init; }
}
