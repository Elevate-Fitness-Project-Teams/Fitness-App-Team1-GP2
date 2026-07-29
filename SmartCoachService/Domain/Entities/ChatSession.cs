using SmartCoachService.Common.Abstractions;

namespace SmartCoachService.Domain.Entities;

public sealed class ChatSession : BaseEntity
{
    public Guid UserId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
