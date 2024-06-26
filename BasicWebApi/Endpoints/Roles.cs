﻿using Application.Identity;
using Application.Identity.Roles;
using Carter;
using Infrastructure.Auth.Permissions;

namespace BasicWebApi.Api.Endpoints;

public class Roles : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/roles");

        group.RequireAuthorization();
        group.WithTags("Roles");

        group.MapGet("/",
            [MustHavePermission(PermissionAction.Search, PermissionResource.Roles)]
        async (IRoleService roleService, CancellationToken cancellationToken) =>
            {
                return await roleService.GetListAsync(cancellationToken);
            });

        group.MapGet("/{id}",
            [MustHavePermission(PermissionAction.View, PermissionResource.Roles)]
        async (string id, IRoleService roleService, CancellationToken cancellationToken) =>
            {
                return await roleService.GetByIdAsync(id, cancellationToken);
            });

        group.MapGet("/{id}/permissions",
            [MustHavePermission(PermissionAction.View, PermissionResource.Roles)]
        async (string id, IRoleService roleService, CancellationToken cancellationToken) =>
            {
                return await roleService.GetByIdWithPermissionsAsync(id, cancellationToken);
            });

        group.MapPut("/{id}/permissions",
            [MustHavePermission(PermissionAction.View, PermissionResource.Roles)]
        async (string id, UpdateRolePermissionsRequest request, IRoleService roleService, CancellationToken cancellationToken) =>
            {
                //if (id != request.RoleId)
                //{
                //    return Result.Fail("Invalid Role id");
                //}
                return await roleService.UpdatePermissionsAsync(request, cancellationToken);
            });

        group.MapPost("/",
            [MustHavePermission(PermissionAction.Create, PermissionResource.Roles)]
        async (CreateOrUpdateRoleRequest request, IRoleService roleService) =>
        {
            return await roleService.CreateOrUpdateAsync(request);
        });

        group.MapDelete("/",
            [MustHavePermission(PermissionAction.Delete, PermissionResource.Roles)]
        async (string roleId, IRoleService roleService) =>
        {
            return await roleService.DeleteAsync(roleId);
        });
    }
}