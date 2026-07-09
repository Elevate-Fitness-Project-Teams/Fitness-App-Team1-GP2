using MediatR;
using NotificationService.Application.Common;

namespace NotificationService.Application.Notifications.MarkNotificationAsRead;

public sealed record MarkNotificationAsReadCommand(
    int NotificationId,
    int UserId
) : IRequest<RequestResult<MarkNotificationAsReadResponse>>;