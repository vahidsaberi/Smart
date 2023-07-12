using Base.Infrastructure.Identity;
using Base.Infrastructure.MultiTenancy;
using Base.Infrastructure.Persistence.Initialization;
using Base.Shared.Authorization;
using Base.Shared.Multitenancy;
using Identity.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Persistence.Initialization;

internal class IdentityDbSeeder
{
    private readonly SmartTenantInfo _currentTenant;
    private readonly ILogger<IdentityDbSeeder> _logger;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly CustomSeederRunner _seederRunner;
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityDbSeeder(SmartTenantInfo currentTenant, RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager, CustomSeederRunner seederRunner, ILogger<IdentityDbSeeder> logger)
    {
        _currentTenant = currentTenant;
        _roleManager = roleManager;
        _userManager = userManager;
        _seederRunner = seederRunner;
        _logger = logger;
    }

    public async Task SeedDatabaseAsync(IdentityDbContext dbContext, CancellationToken cancellationToken)
    {
        await SeedRolesAsync(dbContext);
        await SeedAdminUserAsync();
        await _seederRunner.RunSeedersAsync(cancellationToken);
    }

    private async Task SeedRolesAsync(IdentityDbContext dbContext)
    {
        foreach (var roleName in SmartRoles.DefaultRoles)
        {
            var temp = await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName);
            if (temp is not ApplicationRole role)
            {
                // Create the role
                _logger.LogInformation("Seeding {role} Role for '{tenantId}' Tenant.", roleName, _currentTenant.Id);
                role = new ApplicationRole(roleName, $"{roleName} Role for {_currentTenant.Id} Tenant");
                await _roleManager.CreateAsync(role);
            }

            // Assign permissions
            if (roleName == SmartRoles.Basic)
            {
                await AssignPermissionsToRoleAsync(dbContext, SmartPermissions.Basic, role);
            }
            else if (roleName == SmartRoles.Admin)
            {
                await AssignPermissionsToRoleAsync(dbContext, SmartPermissions.Admin, role);

                if (_currentTenant.Id == MultitenancyConstants.Root.Id)
                    await AssignPermissionsToRoleAsync(dbContext, SmartPermissions.Root, role);
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(IdentityDbContext dbContext,
        IReadOnlyList<SmartPermission> permissions, ApplicationRole role)
    {
        var currentClaims = await _roleManager.GetClaimsAsync(role);
        foreach (var permission in permissions)
            if (!currentClaims.Any(c => c.Type == SmartClaims.Permission && c.Value == permission.Name))
            {
                _logger.LogInformation("Seeding {role} Permission '{permission}' for '{tenantId}' Tenant.", role.Name,
                    permission.Name, _currentTenant.Id);
                dbContext.RoleClaims.Add(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = SmartClaims.Permission,
                    ClaimValue = permission.Name,
                    CreatedBy = "ApplicationDbSeeder"
                });
                await dbContext.SaveChangesAsync();
            }
    }

    private async Task SeedAdminUserAsync()
    {
        if (string.IsNullOrWhiteSpace(_currentTenant.Id) ||
            string.IsNullOrWhiteSpace(_currentTenant.AdminEmail)) return;

        if (await _userManager.Users.FirstOrDefaultAsync(u => u.Email == _currentTenant.AdminEmail)
            is not ApplicationUser adminUser)
        {
            var adminUserName = $"{_currentTenant.Id.Trim()}.{SmartRoles.Admin}".ToLowerInvariant();
            adminUser = new ApplicationUser
            {
                FirstName = _currentTenant.Id.Trim().ToLowerInvariant(),
                LastName = SmartRoles.Admin,
                Email = _currentTenant.AdminEmail,
                UserName = adminUserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = _currentTenant.AdminEmail?.ToUpperInvariant(),
                NormalizedUserName = adminUserName.ToUpperInvariant(),
                IsActive = true
            };

            _logger.LogInformation("Seeding Default Admin User for '{tenantId}' Tenant.", _currentTenant.Id);
            var password = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, MultitenancyConstants.DefaultPassword);
            await _userManager.CreateAsync(adminUser);
        }

        // Assign role to user
        if (!await _userManager.IsInRoleAsync(adminUser, SmartRoles.Admin))
        {
            _logger.LogInformation("Assigning Admin Role to Admin User for '{tenantId}' Tenant.", _currentTenant.Id);
            await _userManager.AddToRoleAsync(adminUser, SmartRoles.Admin);
        }
    }
}