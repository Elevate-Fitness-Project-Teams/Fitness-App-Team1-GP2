namespace SmartCoachService.Common.Abstractions;

public interface ICurrentUserService
{
    Guid UserId { get; }
    bool IsPremium { get; }
}
