using Application.Identity;
using Infrastructure.Auth;
using System.Security.Claims;

namespace Infrastructure.Identity.Services;

public class CurrentUser : ICurrentUser
{
    private ClaimsPrincipal? _user;

    public string? Name => _user?.Identity?.Name;

    private Guid _userId = Guid.Empty;

    public Guid GetUserId()
    {
        throw new NotImplementedException();
    }

    public string GetUserEmail() => _user?.GetEmail() ?? string.Empty;

    //public Guid GetUserId() =>
    //    IsAuthenticated()
    //        ? Guid.Parse(_user?.GetUserId() ?? Guid.Empty.ToString())
    //        : _userId;
}