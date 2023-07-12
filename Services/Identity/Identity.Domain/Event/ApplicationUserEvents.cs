using Base.Domain.Common.Contracts;

namespace Identity.Domain.Event;

public class ApplicationUserEvent : DomainEvent
{
    protected ApplicationUserEvent(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; set; } = default!;
}

public class ApplicationUserCreatedEvent : ApplicationUserEvent
{
    public ApplicationUserCreatedEvent(string userId) : base(userId)
    {
    }
}

public class ApplicationUserUpdatedEvent : ApplicationUserEvent
{
    public ApplicationUserUpdatedEvent(string userId, bool rolesUpdated = false) : base(userId)
    {
        RolesUpdated = rolesUpdated;
    }

    public bool RolesUpdated { get; set; }
}