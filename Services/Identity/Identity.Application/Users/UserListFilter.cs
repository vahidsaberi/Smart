using Base.Application.Common.Models;

namespace Identity.Application.Users;

public class UserListFilter : PaginationFilter
{
    public bool? IsActive { get; set; }
}