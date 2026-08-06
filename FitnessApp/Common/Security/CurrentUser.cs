using System.Security.Claims;

namespace FitnessApp.Common.Security;

public class CurrentUser : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public int UserId
    {
        get
        {
            var userId =
                _httpContextAccessor
                .HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);


            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();


            return int.Parse(userId);
        }
    }
}