using Application.Identity;
using Infrastructure.Auth;
using Infrastructure.Auth.Permissions;
using Infrastructure.Identity;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

internal class ApplicationDbSeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ApplicationDbSeeder> _logger;
    private readonly IKeyCloakService _keyCloakService;

    public ApplicationDbSeeder(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<ApplicationDbSeeder> logger,
        IKeyCloakService keyCloakService)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
        _keyCloakService = keyCloakService;
    }

    public async Task SeedDatabaseAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        await SeedRolesAsync(dbContext);
        await SeedAdminUserAsync();
    }

    private async Task SeedRolesAsync(ApplicationDbContext dbContext)
    {
        foreach (string roleName in Roles.DefaultRoles)
        {
            if (await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName)
                is not ApplicationRole role)
            {
                // Create the role
                _logger.LogInformation("Seeding {role} Role", roleName);
                role = new ApplicationRole(roleName, $"{roleName} Role");
                await _roleManager.CreateAsync(role);
            }

            // Assign permissions
            if (roleName == Roles.Basic)
            {
                await AssignPermissionsToRoleAsync(dbContext, Permissions.Basic, role);
            }
            else if (roleName == Roles.Admin)
            {
                await AssignPermissionsToRoleAsync(dbContext, Permissions.Admin, role);
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(ApplicationDbContext dbContext, IReadOnlyList<Permission> permissions, ApplicationRole role)
    {
        var currentClaims = await _roleManager.GetClaimsAsync(role);
        foreach (var permission in permissions)
        {
            if (!currentClaims.Any(c => c.Type == Claims.Permission && c.Value == permission.Name))
            {
                _logger.LogInformation("Seeding {role} Permission '{permission}'.", role.Name, permission.Name);
                dbContext.RoleClaims.Add(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = Claims.Permission,
                    ClaimValue = permission.Name,
                    CreatedBy = "ApplicationDbSeeder"
                });
                await dbContext.SaveChangesAsync();
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        string adminEmail = "admin@mail.com";
        if (await _userManager.Users.FirstOrDefaultAsync(u => u.Email == adminEmail)
            is not ApplicationUser adminUser)
        {
            string adminUserName = $"{Roles.Admin}".ToLowerInvariant();
            string keyCloakId = await _keyCloakService.CreateUser(adminUserName, adminEmail, "123qweassd");
            adminUser = new ApplicationUser
            {
                FirstName = "admin",
                LastName = Roles.Admin,
                Email = adminEmail,
                UserName = adminUserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = adminEmail.ToUpperInvariant(),
                NormalizedUserName = adminUserName.ToUpperInvariant(),
                IsActive = true,
                KeyCloakId = keyCloakId
            };

            _logger.LogInformation("Seeding Default Admin User.");
            var password = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, "123qweassd");
            await _userManager.CreateAsync(adminUser);
        }

        // Assign role to user
        if (!await _userManager.IsInRoleAsync(adminUser, Roles.Admin))
        {
            _logger.LogInformation("Assigning Admin Role to Admin User.");
            await _userManager.AddToRoleAsync(adminUser, Roles.Admin);
        }
    }
}