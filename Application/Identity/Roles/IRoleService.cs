using Application.Common.Interfaces;
using System.Threading;

namespace Application.Identity.Roles;

public interface IRoleService : ITransientService
{
    Task<List<RoleDto>> GetListAsync(CancellationToken cancellationToken);

    Task<RoleDto> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<RoleDto> GetByIdWithPermissionsAsync(string roleId, CancellationToken cancellationToken);

    Task<string> CreateOrUpdateAsync(CreateOrUpdateRoleRequest request);

    Task<string> DeleteAsync(string id);
}