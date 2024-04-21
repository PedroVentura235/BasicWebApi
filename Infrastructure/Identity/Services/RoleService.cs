using Application.Exceptions;
using Application.Identity.Roles;
using Infrastructure.Auth;
using Infrastructure.Auth.Permissions;
using Infrastructure.Persistence.Context;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity.Services;

internal class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public RoleService(
        RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext db)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _db = db;
    }

    public async Task<List<RoleDto>> GetListAsync(CancellationToken cancellationToken) =>
        await _roleManager.Roles
        .AsNoTracking()
        .ProjectToType<RoleDto>()
        .ToListAsync(cancellationToken);

    public async Task<RoleDto> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await _db.Roles.SingleOrDefaultAsync(x => x.Id == id, cancellationToken) is { } role
            ? role.Adapt<RoleDto>()
            : throw new NotFoundException("Role Not Found");

    public async Task<RoleDto> GetByIdWithPermissionsAsync(string roleId, CancellationToken cancellationToken)
    {
        RoleDto role = await GetByIdAsync(roleId, cancellationToken);

        role.Permissions = await _db.RoleClaims
            .Where(c => c.RoleId == roleId && c.ClaimType == Claims.Permission)
            .Select(c => c.ClaimValue!)
            .ToListAsync(cancellationToken);

        return role;
    }

    public async Task<string> CreateOrUpdateAsync(CreateOrUpdateRoleRequest request)
    {
        if (string.IsNullOrEmpty(request.Id))
        {
            // Create a new role.
            ApplicationRole role = new(request.Name, request.Description);
            IdentityResult result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                throw new InternalServerException("Register role failed", result.GetErrors());
            }

            //await _events.PublishAsync(new ApplicationRoleCreatedEvent(role.Id, role.Name!));

            return string.Format("Role {0} Created.", request.Name);
        }
        else
        {
            // Update an existing role.
            ApplicationRole role = await _roleManager.FindByIdAsync(request.Id);

            _ = role ?? throw new NotFoundException("Role Not Found");

            if (Roles.IsDefault(role.Name!))
            {
                throw new ConflictException(string.Format("Not allowed to modify {0} Role.", role.Name));
            }

            role.Name = request.Name;
            role.NormalizedName = request.Name.ToUpperInvariant();
            role.Description = request.Description;
            IdentityResult result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                throw new InternalServerException("Update role failed", result.GetErrors());
            }

            //await _events.PublishAsync(new ApplicationRoleUpdatedEvent(role.Id, role.Name));

            return string.Format("Role {0} Updated.", role.Name);
        }
    }

    public async Task<string> DeleteAsync(string id)
    {
        ApplicationRole role = await _roleManager.FindByIdAsync(id);

        _ = role ?? throw new NotFoundException("Role Not Found");

        if (Roles.IsDefault(role.Name!))
        {
            throw new ConflictException(string.Format("Not allowed to delete {0} Role.", role.Name));
        }

        if ((await _userManager.GetUsersInRoleAsync(role.Name!)).Count > 0)
        {
            throw new ConflictException(string.Format("Not allowed to delete {0} Role as it is being used.", role.Name));
        }

        await _roleManager.DeleteAsync(role);

        //await _events.PublishAsync(new ApplicationRoleDeletedEvent(role.Id, role.Name!));

        return string.Format("Role {0} Deleted.", role.Name);
    }
}