using NutritionService.Common.Abstractions;
using System.Security.Claims;

namespace NutritionService.Infrastructure;

public sealed class CurrentUserService : ICurrentUserService
{
    public Guid UserId { get; }
    public bool IsPremium { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var idClaim = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? user?.FindFirstValue("sub");
        UserId = Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
        IsPremium = user?.FindFirstValue("subscriptionTier") == "Premium";
    }
}
