using Application.Identity;
using BasicWebApi.Infrastructure.Auth;
using Infrastructure.Auth;
using System.Security.Claims;

namespace Infrastructure.Identity.Services;

public class CurrentUser : ICurrentUser, ICurrentUserInitializer
{
    private ClaimsPrincipal? _user;

    public string? Name => _user?.Identity?.Name;

    private Guid _userId = Guid.Empty;

    public Guid GetUserId()
    {
        throw new NotImplementedException();
    }

    public string GetUserEmail() => _user?.GetEmail() ?? string.Empty;

    public void SetCurrentUser(ClaimsPrincipal user)
    {
        if (_user != null)
        {
            throw new Exception("Method reserved for in-scope initialization");
        }

        _user = user;
    }
}