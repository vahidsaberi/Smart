using System.Collections.ObjectModel;

namespace Base.Shared.Authorization;

public static class SmartRoles
{
    public const string Admin = nameof(Admin);
    public const string Basic = nameof(Basic);

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[]
    {
        Admin,
        Basic
    });

    public static bool IsDefault(string roleName)
    {
        return DefaultRoles.Any(_ => _ == roleName);
    }
}