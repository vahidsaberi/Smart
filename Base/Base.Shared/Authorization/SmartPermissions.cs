using System.Collections.ObjectModel;

namespace Base.Shared.Authorization;

public static class SmartAction
{
    public const string View = nameof(View);
    public const string Search = nameof(Search);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string Export = nameof(Export);
    public const string Generate = nameof(Generate);
    public const string Clean = nameof(Clean);
    public const string UpgradeSubscription = nameof(UpgradeSubscription);
}

public static class SmartResource
{
    public const string Tenants = nameof(Tenants);
    public const string Dashboard = nameof(Dashboard);
    public const string Hangfire = nameof(Hangfire);
    public const string Users = nameof(Users);
    public const string UserRoles = nameof(UserRoles);
    public const string Roles = nameof(Roles);
    public const string RoleClaims = nameof(RoleClaims);
}

public record SmartPermission(string Description, string Action, string Resource, bool IsBasic = false,
    bool IsRoot = false)
{
    public string Name => NameFor(Action, Resource);

    public static string NameFor(string action, string resource)
    {
        return $"Permissions.{resource}.{action}";
    }
}

public static class SmartPermissions
{
    private static readonly SmartPermission[] _all =
    {
        new("View Dashboard", SmartAction.View, SmartResource.Dashboard),

        new("View Hangfire", SmartAction.View, SmartResource.Hangfire),

        new("View Users", SmartAction.View, SmartResource.Users),
        new("Search Users", SmartAction.Search, SmartResource.Users),
        new("Create Users", SmartAction.Create, SmartResource.Users),
        new("Update Users", SmartAction.Update, SmartResource.Users),
        new("Delete Users", SmartAction.Delete, SmartResource.Users),
        new("Export Users", SmartAction.Export, SmartResource.Users),

        new("View UserRoles", SmartAction.View, SmartResource.UserRoles),
        new("Update UserRoles", SmartAction.Update, SmartResource.UserRoles),

        new("View Roles", SmartAction.View, SmartResource.Roles),
        new("Create Roles", SmartAction.Create, SmartResource.Roles),
        new("Update Roles", SmartAction.Update, SmartResource.Roles),
        new("Delete Roles", SmartAction.Delete, SmartResource.Roles),

        new("View RoleClaims", SmartAction.View, SmartResource.RoleClaims),
        new("Update RoleClaims", SmartAction.Update, SmartResource.RoleClaims),

        new("View Tenants", SmartAction.View, SmartResource.Tenants, IsRoot: true),
        new("Create Tenants", SmartAction.Create, SmartResource.Tenants, IsRoot: true),
        new("Update Tenants", SmartAction.Update, SmartResource.Tenants, IsRoot: true),
        new("Upgrade Tenant Subscription", SmartAction.UpgradeSubscription, SmartResource.Tenants, IsRoot: true)
    };

    public static IReadOnlyList<SmartPermission> All { get; } = new ReadOnlyCollection<SmartPermission>(_all);

    public static IReadOnlyList<SmartPermission> Root { get; } =
        new ReadOnlyCollection<SmartPermission>(_all.Where(_ => _.IsRoot).ToArray());

    public static IReadOnlyList<SmartPermission> Admin { get; } =
        new ReadOnlyCollection<SmartPermission>(_all.Where(_ => !_.IsRoot).ToArray());

    public static IReadOnlyList<SmartPermission> Basic { get; } =
        new ReadOnlyCollection<SmartPermission>(_all.Where(_ => _.IsBasic).ToArray());
}