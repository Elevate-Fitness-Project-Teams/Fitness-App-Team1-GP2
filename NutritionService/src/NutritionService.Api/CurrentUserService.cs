using System.Security.Claims;
using NutritionCurrentUserService = NutritionService.Application.Common.Abstractions.ICurrentUserService;
using SmartCoachCurrentUserService = SmartCoachService.Application.Common.Interfaces.ICurrentUserService;

namespace NutritionService.Api;

public class CurrentUserService : NutritionCurrentUserService, SmartCoachCurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    public Guid UserId
    {
        get
        {
            var raw = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;

            return Guid.TryParse(raw, out var id)
                ? id
                : throw new UnauthorizedAccessException("Caller userId claim is missing or invalid.");
        }
    }

    public bool IsPremium
    {
        get
        {
            var tier = _httpContextAccessor.HttpContext?.User.FindFirst("tier")?.Value;
            return string.Equals(tier, "premium", StringComparison.OrdinalIgnoreCase);
        }
    }
}
