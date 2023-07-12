using Microsoft.AspNetCore.Identity;

namespace Base.Infrastructure.Identity;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole(string name, string? description = null)
        : base(name)
    {
        Description = description;
        NormalizedName = name.ToUpperInvariant();
    }

    public string? Description { get; set; }
}