using Base.Domain.Common.Contracts;

namespace Identity.Domain.Event;

public abstract class ApplicationRoleEvent : DomainEvent
{
    protected ApplicationRoleEvent(string roleId, string roleName)
    {
        (RoleId, RoleName) = (roleId, roleName);
    }

    public string RoleId { get; set; } = default!;
    public string RoleName { get; set; } = default!;
}

public class ApplicationRoleCreatedEvent : ApplicationRoleEvent
{
    public ApplicationRoleCreatedEvent(string roleId, string roleName) : base(roleId, roleName)
    {
    }
}

public class ApplicationRoleUpdatedEvent : ApplicationRoleEvent
{
    public ApplicationRoleUpdatedEvent(string roleId, string roleName, bool permissionsUpdated = false)
        : base(roleId, roleName)
    {
        PermissionsUpdated = permissionsUpdated;
    }

    public bool PermissionsUpdated { get; set; }
}

public class ApplicationRoleDeletedEvent : ApplicationRoleEvent
{
    public ApplicationRoleDeletedEvent(string roleId, string roleName)
        : base(roleId, roleName)
    {
    }

    public bool PermissionsUpdated { get; set; }
}