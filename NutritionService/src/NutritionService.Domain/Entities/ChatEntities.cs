using SmartCoachService.Domain.Enums;

namespace SmartCoachService.Domain.Entities;

public class ChatSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime LastActivityAtUtc { get; set; }

    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}

public class ChatMessage
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public ChatSession Session { get; set; } = default!;

    public SenderType Sender { get; set; }
    public string Content { get; set; } = default!;
    public DateTime SentAtUtc { get; set; }
}
