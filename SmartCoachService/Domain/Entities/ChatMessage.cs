using SmartCoachService.Common.Abstractions;
using SmartCoachService.Domain.Enums;

namespace SmartCoachService.Domain.Entities;

public sealed class ChatMessage : BaseEntity
{
    public Guid SessionId { get; set; }
    public ChatSession Session { get; set; } = default!;

    public ChatSender Sender { get; set; }
    public string Content { get; set; } = default!;

    // Only populated on AI messages.
    public List<string> FollowUpSuggestions { get; set; } = new();
}
