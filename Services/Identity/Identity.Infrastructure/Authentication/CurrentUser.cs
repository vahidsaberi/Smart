using System.Security.Claims;
using Base.Application.Common.Interfaces;
using Base.Infrastructure.Authentication;
using Base.Shared.Authorization;

namespace Identity.Infrastructure.Authentication;

public class CurrentUser : ICurrentUser, ICurrentUserInitializer
{
    private ClaimsPrincipal? _user;

    private Guid _userId = Guid.Empty;

    public string? Name => _user?.Identity?.Name;

    public Guid GetUserId()
    {
        return IsAuthenticated()
            ? Guid.Parse(_user?.GetUserId() ?? Guid.Empty.ToString())
            : _userId;
    }

    public string? GetUserEmail()
    {
        return IsAuthenticated()
            ? _user!.GetEmail()
            : string.Empty;
    }

    public bool IsAuthenticated()
    {
        return _user?.Identity?.IsAuthenticated is true;
    }

    public bool IsInRole(string role)
    {
        return _user?.IsInRole(role) is true;
    }

    public IEnumerable<Claim>? GetUserClaims()
    {
        return _user?.Claims;
    }

    public string? GetTenant()
    {
        return IsAuthenticated() ? _user?.GetTenant() : string.Empty;
    }

    public void SetCurrentUser(ClaimsPrincipal user)
    {
        if (_user != null) throw new Exception("Method reserved for in-scope initialization");

        _user = user;
    }

    public void SetCurrentUserId(string userId)
    {
        if (_userId != Guid.Empty) throw new Exception("Method reserved for in-scope initialization");

        if (!string.IsNullOrEmpty(userId)) _userId = Guid.Parse(userId);
    }
}