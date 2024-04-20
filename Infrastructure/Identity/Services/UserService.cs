using Application.Exceptions;
using Application.Identity;
using Infrastructure.Auth;
using Infrastructure.Auth.Permissions;
using Infrastructure.Identity.KeyCloak;
using Infrastructure.Persistence.Context;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Identity.Services;

public class UserService : IUserService
{
    private readonly IKeyCloakService _keyCloakService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _db;
    //private readonly ICacheService _cache;

    public UserService(
        IKeyCloakService keyCloakService,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext db)
    {
        _keyCloakService = keyCloakService;
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
    }

    public async Task<TokenResponseDto> GetTokenAsync(string username, string password)
    {
        TokenResponseDto keycloakTokenResponseDto = await _keyCloakService.GetTokenAsync(username, password);

        return keycloakTokenResponseDto;
    }

    #region Users

    public async Task<string> CreateUserAsync(CreateUserRequest request)
    {
        string keyCloakId = await _keyCloakService.CreateUser(request.UserName, request.Email, request.Password);
        if (!string.IsNullOrEmpty(keyCloakId))
        {
            //create on database
            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                KeyCloakId = keyCloakId
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, Roles.Basic);
            return keyCloakId;
        }

        return "error";
    }

    public async Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken) =>
       (await _userManager.Users
               .AsNoTracking()
               .ProjectToType<UserDetailsDto>()
               .ToListAsync(cancellationToken));

    public async Task<string> DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        string response = await _keyCloakService.DeleteUserAsync(user.KeyCloakId!);
        if (!String.IsNullOrEmpty(response))
        {
            await _userManager.DeleteAsync(user);
        }
        return id;
    }

    #endregion Users

    #region Permissions

    public async Task<bool> HasPermissionAsync(string userEmail, string permission, CancellationToken cancellationToken)
    {
        //var permissions = await _cache.GetOrSetAsync(
        //    _cacheKeys.GetCacheKey(FSHClaims.Permission, userId),
        //    () => GetPermissionsAsync(userId, cancellationToken),
        //    cancellationToken: cancellationToken);
        List<string> permissions = await GetPermissionsAsync(userEmail, cancellationToken);

        return permissions?.Contains(permission) ?? false;
    }

    public async Task<List<string>> GetPermissionsAsync(string userEmail, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);

        _ = user ?? throw new UnauthorizedException("Authentication Failed.");

        IList<string> userRoles = await _userManager.GetRolesAsync(user);
        List<string> permissions = new();

        foreach (var role in await _roleManager.Roles
            .Where(r => userRoles.Contains(r.Name!))
            .ToListAsync(cancellationToken))
        {
            permissions.AddRange(await _db.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == Claims.Permission)
                .Select(rc => rc.ClaimValue!)
                .ToListAsync(cancellationToken));
        }

        return permissions.Distinct().ToList();
    }

    #endregion Permissions
}