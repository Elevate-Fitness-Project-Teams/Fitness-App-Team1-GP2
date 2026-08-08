using MediatR;
using Microsoft.Extensions.Caching.Memory;
using NotificationService.Application.Common;
using NotificationService.Common;
using NotificationService.Domain.Notifications;

namespace NotificationService.Application.Notifications.MarkNotificationAsRead;

public sealed class MarkNotificationAsReadHandler(
    INotificationRepository notificationRepository,
    IMemoryCache memoryCache)
    : IRequestHandler<MarkNotificationAsReadCommand, RequestResult<MarkNotificationAsReadResponse>>
{
    public async Task<RequestResult<MarkNotificationAsReadResponse>> Handle(
        MarkNotificationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetByIdForUserAsync(
            request.NotificationId,
            request.UserId,
            cancellationToken);

        if (notification is null)
        {
            return RequestResult<MarkNotificationAsReadResponse>
                .Failure(ErrorCode.NotificationNotFound);
        }

        notification.MarkAsRead();

        await notificationRepository.SaveChangesAsync(cancellationToken);

        memoryCache.Remove($"UserNotifications_{request.UserId}");

        var response = new MarkNotificationAsReadResponse
        {
            Id = notification.Id,
            IsRead = notification.IsRead
        };

        return RequestResult<MarkNotificationAsReadResponse>.Success(response);
    }
}