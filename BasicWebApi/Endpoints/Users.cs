using Application.Identity;
using Carter;
using Infrastructure.Auth.Permissions;

namespace BasicWebApi.Api.Endpoints;

public class Users : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/users");

        group.WithTags("Users").WithDescription("Endpoints related to users.");

        group.MapGet("/login", async (string username, string password, IUserService userService) =>
        {
            return await userService.GetTokenAsync(username, password);
        });

        group.MapGet("/",
        [MustHavePermission(PermissionAction.Search, PermissionResource.Users)]
        async (IUserService userService, CancellationToken cancellationToken, ICurrentUser currentUser) =>
        {
            return await userService.GetListAsync(cancellationToken);
        }).RequireAuthorization();

        group.MapPost("/", async (CreateUserRequest request, IUserService userService) =>
        {
            return await userService.CreateUserAsync(request);
        }).RequireAuthorization();

        group.MapDelete("/",
        [MustHavePermission(PermissionAction.Delete, PermissionResource.Users)]
        async (string userId, IUserService userService, CancellationToken cancellationToken) =>
        {
            return await userService.DeleteUserAsync(userId);
        }).RequireAuthorization();
    }
}