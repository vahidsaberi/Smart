using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Identity.Infrastructure.Identity;

internal static class IdentityResultExtensions
{
    public static List<string> GetErrors(this IdentityResult result, IStringLocalizer T)
    {
        return result.Errors.Select(e => T[e.Description].ToString()).ToList();
    }
}