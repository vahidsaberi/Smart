using Base.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Infrastructure.Authentication.Permissions;

public class MustHavePermissionAttribute : AuthorizeAttribute
{
    public MustHavePermissionAttribute(string action, string resource)
    {
        Policy = SmartPermission.NameFor(action, resource);
    }
}