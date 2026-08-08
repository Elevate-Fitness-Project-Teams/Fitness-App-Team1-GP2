using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using NotificationService.Domain.Notifications;

namespace NotificationService.Infrastructure.Messaging;

public sealed class AchievementEarnedConsumer(
    INotificationRepository notificationRepository,
    IMemoryCache memoryCache,
    ILogger<AchievementEarnedConsumer> logger)
    : IConsumer<AchievementEarnedEvent>
{
    public async Task Consume(
        ConsumeContext<AchievementEarnedEvent> context)
    {
        try
        {
            var integrationEvent = context.Message;

            var notification = new InAppNotification(
                integrationEvent.UserId,
                "Achievement unlocked!",
                $"{integrationEvent.AchievementName}: {integrationEvent.Description}",
                NotificationType.AchievementAlert);

            await notificationRepository.AddAsync(
                notification,
                context.CancellationToken);

            await notificationRepository.SaveChangesAsync(
                context.CancellationToken);


            memoryCache.Remove(
                $"UserNotifications_{integrationEvent.UserId}");


            logger.LogInformation(
                "Achievement notification created for user {UserId}",
                integrationEvent.UserId);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error while consuming AchievementEarnedEvent");

            throw;
        }
    }
}