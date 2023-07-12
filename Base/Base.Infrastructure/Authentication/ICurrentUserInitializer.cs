using System.Security.Claims;

namespace Base.Infrastructure.Authentication;

public interface ICurrentUserInitializer
{
    void SetCurrentUser(ClaimsPrincipal user);

    void SetCurrentUserId(string userId);
}