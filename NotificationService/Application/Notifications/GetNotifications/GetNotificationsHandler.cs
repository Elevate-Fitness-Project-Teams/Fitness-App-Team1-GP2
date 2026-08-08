using MediatR;
using Microsoft.Extensions.Caching.Memory;
using NotificationService.Application.Common;
using NotificationService.Domain.Notifications;

namespace NotificationService.Application.Notifications.GetNotifications;

public sealed class GetNotificationsHandler(
    INotificationRepository notificationRepository,
    IMemoryCache memoryCache)
    : IRequestHandler<GetNotificationsQuery, RequestResult<List<GetNotificationsResponse>>>
{
    public async Task<RequestResult<List<GetNotificationsResponse>>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"UserNotifications_{request.UserId}";

        if (memoryCache.TryGetValue(
                cacheKey,
                out List<GetNotificationsResponse>? cachedNotifications))
        {
            return RequestResult<List<GetNotificationsResponse>>
                .Success(cachedNotifications!);
        }

        var notifications = await notificationRepository.GetUserNotificationsAsync(
            request.UserId,
            cancellationToken);

        var response = notifications
            .Select(notification => new GetNotificationsResponse
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                IsRead = notification.IsRead,
                SentAt = notification.SentAt
            })
            .ToList();

        memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(5));

        return RequestResult<List<GetNotificationsResponse>>.Success(response);
    }
}