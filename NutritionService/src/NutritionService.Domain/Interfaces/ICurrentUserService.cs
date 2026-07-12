namespace SmartCoachService.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    bool IsPremium { get; }
}
