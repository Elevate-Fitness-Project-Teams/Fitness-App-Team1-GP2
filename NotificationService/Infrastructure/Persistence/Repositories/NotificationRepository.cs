using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Notifications;

namespace NotificationService.Infrastructure.Persistence.Repositories;

public sealed class NotificationRepository(NotificationDbContext context)
    : INotificationRepository
{
    public async Task<List<InAppNotification>> GetUserNotificationsAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return await context.InAppNotifications
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.IsRead)
            .ThenByDescending(x => x.SentAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<InAppNotification?> GetByIdForUserAsync(
        int notificationId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        return await context.InAppNotifications
            .FirstOrDefaultAsync(
                x => x.Id == notificationId && x.UserId == userId,
                cancellationToken);
    }

    public async Task AddAsync(
        InAppNotification notification,
        CancellationToken cancellationToken = default)
    {
        await context.InAppNotifications.AddAsync(notification, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}