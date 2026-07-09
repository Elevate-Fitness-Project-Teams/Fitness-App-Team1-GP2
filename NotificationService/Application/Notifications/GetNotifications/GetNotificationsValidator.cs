using FluentValidation;

namespace NotificationService.Application.Notifications.GetNotifications;

public sealed class GetNotificationsValidator
    : AbstractValidator<GetNotificationsQuery>
{
    public GetNotificationsValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User id must be greater than 0.");
    }
}