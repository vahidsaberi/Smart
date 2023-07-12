using System.Security.Claims;

namespace Base.Shared.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value;
    }

    public static string? GetTenant(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(SmartClaims.Tenant)?.Value;
    }

    public static string? GetFullname(this ClaimsPrincipal principal)
    {
        return principal?.FindFirst(SmartClaims.Fullname)?.Value;
    }

    public static string? GetFirstName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Name)?.Value;
    }

    public static string? GetSurname(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Surname)?.Value;
    }

    public static string? GetPhoneNumber(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.MobilePhone)?.Value;
    }

    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public static string? GetImageUrl(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(SmartClaims.ImageUrl)?.Value;
    }

    public static DateTimeOffset GetExpiration(this ClaimsPrincipal principal)
    {
        return DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(principal.FindFirst(SmartClaims.Expiration)));
    }

    public static string? FindFirstValue(this ClaimsPrincipal principal, string claimType)
    {
        return principal is null
            ? throw new ArgumentNullException(nameof(principal))
            : principal.FindFirst(claimType)?.Value;
    }
}