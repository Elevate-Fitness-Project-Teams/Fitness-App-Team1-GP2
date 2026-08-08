namespace SmartCoachService.Infrastructure.Messaging.Events;

public sealed record ChatMessageSentEvent(Guid UserId, Guid SessionId, DateTime SentAtUtc);
