namespace NotificationService.Domain.Notifications;

public interface INotificationRepository
{
    Task<List<InAppNotification>> GetUserNotificationsAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<InAppNotification?> GetByIdForUserAsync(
        int notificationId,
        int userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        InAppNotification notification,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}