using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Identity;

public interface IUserService
{
    Task<TokenResponseDto> GetTokenAsync(string username, string password);

    Task<string> CreateUserAsync(CreateUserRequest request);

    Task<string> DeleteUserAsync(string id);

    Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken = default);

    Task<UserDetailsDto> GetAsync(string userEmail, CancellationToken cancellationToken);

    Task<bool> HasPermissionAsync(string userEmail, string permission, CancellationToken cancellationToken = default);

    Task<List<string>> GetPermissionsAsync(string userEmail, CancellationToken cancellationToken);
}