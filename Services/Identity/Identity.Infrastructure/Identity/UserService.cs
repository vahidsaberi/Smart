using Ardalis.Specification.EntityFrameworkCore;
using Base.Application.Common.Caching;
using Base.Application.Common.Events;
using Base.Application.Common.Exceptions;
using Base.Application.Common.FileStorage;
using Base.Application.Common.Job;
using Base.Application.Common.Mailing;
using Base.Application.Common.Models;
using Base.Application.Common.Specification;
using Base.Infrastructure.Identity;
using Base.Shared.Authorization;
using Finbuckle.MultiTenant;
using Identity.Application.Users;
using Identity.Domain.Event;
using Identity.Infrastructure.Authentication;
using Identity.Infrastructure.Persistence.Context;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Identity;

internal partial class UserService : IUserService
{
    private readonly ICacheService _cache;
    private readonly ICacheKeyService _cacheKeys;
    private readonly ITenantInfo _currentTenant;
    private readonly IdentityDbContext _db;
    private readonly IEventPublisher _events;
    private readonly IFileStorageService _fileStorage;
    private readonly IJobService _jobService;
    private readonly IMailService _mailService;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SecuritySettings _securitySettings;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IStringLocalizer _t;
    private readonly IEmailTemplateService _templateService;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IdentityDbContext db,
        IStringLocalizer<UserService> localizer,
        IJobService jobService,
        IMailService mailService,
        IEmailTemplateService templateService,
        IFileStorageService fileStorage,
        IEventPublisher events,
        ICacheService cache,
        ICacheKeyService cacheKeys,
        ITenantInfo currentTenant,
        IOptions<SecuritySettings> securitySettings)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _t = localizer;
        _jobService = jobService;
        _mailService = mailService;
        _templateService = templateService;
        _fileStorage = fileStorage;
        _events = events;
        _cache = cache;
        _cacheKeys = cacheKeys;
        _currentTenant = currentTenant;
        _securitySettings = securitySettings.Value;
    }

    public async Task<PaginationResponse<UserDetailsDto>> SearchAsync(UserListFilter filter,
        CancellationToken cancellationToken)
    {
        var spec = new EntitiesByPaginationFilterSpec<ApplicationUser>(filter);

        var users = await _userManager.Users
            .WithSpecification(spec)
            .ProjectToType<UserDetailsDto>()
            .ToListAsync(cancellationToken);
        var count = await _userManager.Users
            .CountAsync(cancellationToken);

        return new PaginationResponse<UserDetailsDto>(users, count, filter.PageNumber, filter.PageSize);
    }

    public async Task<bool> ExistsWithNameAsync(string name)
    {
        EnsureValidTenant();
        return await _userManager.FindByNameAsync(name) is not null;
    }

    public async Task<bool> ExistsWithEmailAsync(string email, string? exceptId = null)
    {
        EnsureValidTenant();
        return await _userManager.FindByEmailAsync(email.Normalize()) is ApplicationUser user && user.Id != exceptId;
    }

    public async Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, string? exceptId = null)
    {
        EnsureValidTenant();
        return await _userManager.Users.FirstOrDefaultAsync(x =>
            x.PhoneNumber == phoneNumber) is ApplicationUser user && user.Id != exceptId;
    }

    public async Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken)
    {
        return (await _userManager.Users
                .AsNoTracking()
                .ToListAsync(cancellationToken))
            .Adapt<List<UserDetailsDto>>();
    }

    public Task<int> GetCountAsync(CancellationToken cancellationToken)
    {
        return _userManager.Users.AsNoTracking().CountAsync(cancellationToken);
    }

    public async Task<UserDetailsDto> GetAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        return user.Adapt<UserDetailsDto>();
    }

    public async Task ToggleStatusAsync(ToggleUserStatusRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        var isAdmin = await _userManager.IsInRoleAsync(user, SmartRoles.Admin);
        if (isAdmin) throw new ConflictException(_t["Administrators Profile's Status cannot be toggled"]);

        user.IsActive = request.ActivateUser;

        await _userManager.UpdateAsync(user);

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));
    }

    private void EnsureValidTenant()
    {
        if (string.IsNullOrWhiteSpace(_currentTenant?.Id)) throw new UnauthorizedException(_t["Invalid Tenant."]);
    }
}