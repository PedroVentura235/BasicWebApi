using Application.Identity;
using Carter;
using Infrastructure.Auth.Permissions;
using System.Security.Claims;

namespace BasicWebApi.Api.Endpoints;

public class Personal : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/personal");

        group.WithTags("Personal")
            .WithDescription("Endpoints related to current user.")
            .RequireAuthorization();

        group.MapGet("/",
        [MustHavePermission(PermissionAction.Search, PermissionResource.Users)]
        async (IUserService userService, CancellationToken cancellationToken, ICurrentUser user, ClaimsPrincipal principal) =>
        {
            return await userService.GetAsync(user.GetUserEmail(), cancellationToken);
        });

        group.MapGet("/permissions",
       [MustHavePermission(PermissionAction.Search, PermissionResource.Users)]
        async (IUserService userService, CancellationToken cancellationToken, ICurrentUser user, ClaimsPrincipal principal) =>
       {
           return await userService.GetPermissionsAsync(user.GetUserEmail(), cancellationToken);
       });
    }
}