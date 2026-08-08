using MediatR;
using NotificationService.Application.Common;

namespace NotificationService.Application.Notifications.GetNotifications;

public sealed record GetNotificationsQuery(int UserId)
    : IRequest<RequestResult<List<GetNotificationsResponse>>>;