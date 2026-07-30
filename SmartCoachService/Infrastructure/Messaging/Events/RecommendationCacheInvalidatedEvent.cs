namespace SmartCoachService.Infrastructure.Messaging.Events;

/// <summary>
/// Published whenever another service (Profile/FCE/Workout/Nutrition/Progress) changes
/// data that affects the Home Feed, so a consumer here can proactively expire RecommendationCache.
/// Also used as the outbound event this service raises after a fresh aggregation.
/// </summary>
public sealed record RecommendationCacheInvalidatedEvent(Guid UserId, DateTime InvalidatedAtUtc);
