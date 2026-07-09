namespace NotificationService.Infrastructure.Messaging;

public sealed record AchievementEarnedIntegrationEvent(
    int UserId,
    int AchievementId,
    string AchievementName,
    string Description,
    DateTime EarnedAt
);