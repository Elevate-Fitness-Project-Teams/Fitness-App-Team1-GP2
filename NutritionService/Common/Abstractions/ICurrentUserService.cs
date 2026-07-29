namespace NutritionService.Common.Abstractions;

/// <summary>Resolves the authenticated caller's UserId from the validated JWT (set by the API Gateway / AuthenticationService).</summary>
public interface ICurrentUserService
{
    Guid UserId { get; }
    bool IsPremium { get; }
}
