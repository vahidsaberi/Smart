namespace Identity.Application.Users;

public class UserRolesRequest
{
    public List<UserRoleDto> UserRoles { get; set; } = new();
}