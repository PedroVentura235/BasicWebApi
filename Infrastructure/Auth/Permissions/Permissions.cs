using System.Collections.ObjectModel;

namespace Infrastructure.Auth.Permissions;

public static class PermissionAction
{
    public const string View = nameof(View);
    public const string Search = nameof(Search);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string Export = nameof(Export);
    public const string Generate = nameof(Generate);
    public const string Clean = nameof(Clean);
    public const string UpgradeSubscription = nameof(UpgradeSubscription);
}

public static class PermissionResource
{
    public const string Users = nameof(Users);
    public const string Roles = nameof(Roles);
}

public static class Permissions
{
    private static readonly Permission[] _all =
    [
        new("View Users", PermissionAction.View, PermissionResource.Users),
        new("Search Users", PermissionAction.Search, PermissionResource.Users),
        new("Create Users", PermissionAction.Create, PermissionResource.Users),
        new("Update Users", PermissionAction.Update, PermissionResource.Users),
        new("Delete Users", PermissionAction.Delete, PermissionResource.Users),
        new("Export Users", PermissionAction.Export, PermissionResource.Users),
        //new("View UserRoles", PermissionAction.View, PermissionResource.UserRoles),
        //new("Update UserRoles", PermissionAction.Update, PermissionResource.UserRoles),
        new("View Roles", PermissionAction.View, PermissionResource.Roles),
        new("Search Roles", PermissionAction.Search, PermissionResource.Roles),
        new("Create Roles", PermissionAction.Create, PermissionResource.Roles),
        new("Update Roles", PermissionAction.Update, PermissionResource.Roles),
        new("Delete Roles", PermissionAction.Delete, PermissionResource.Roles),
        //new("View RoleClaims", PermissionAction.View, PermissionResource.RoleClaims),
        //new("Update RoleClaims", PermissionAction.Update, PermissionResource.RoleClaims),
        //new("View Products", PermissionAction.View, PermissionResource.Products, IsBasic: true),
        //new("Search Products", PermissionAction.Search, PermissionResource.Products, IsBasic: true),
        //new("Create Products", PermissionAction.Create, PermissionResource.Products),
        //new("Update Products", PermissionAction.Update, PermissionResource.Products),
        //new("Delete Products", PermissionAction.Delete, PermissionResource.Products),
        //new("Export Products", PermissionAction.Export, PermissionResource.Products),
        //new("View Brands", PermissionAction.View, PermissionResource.Brands, IsBasic: true),
        //new("Search Brands", PermissionAction.Search, PermissionResource.Brands, IsBasic: true),
        //new("Create Brands", PermissionAction.Create, PermissionResource.Brands),
        //new("Update Brands", PermissionAction.Update, PermissionResource.Brands),
        //new("Delete Brands", PermissionAction.Delete, PermissionResource.Brands),
        //new("Generate Brands", PermissionAction.Generate, PermissionResource.Brands),
        //new("Clean Brands", PermissionAction.Clean, PermissionResource.Brands),
        //new("View Tenants", PermissionAction.View, PermissionResource.Tenants, IsRoot: true),
        //new("Create Tenants", PermissionAction.Create, PermissionResource.Tenants, IsRoot: true),
        //new("Update Tenants", PermissionAction.Update, PermissionResource.Tenants, IsRoot: true),
        //new("Upgrade Tenant Subscription", PermissionAction.UpgradeSubscription, PermissionResource.Tenants, IsRoot: true)
    ];

    public static IReadOnlyList<Permission> All { get; } = new ReadOnlyCollection<Permission>(_all);
    public static IReadOnlyList<Permission> Root { get; } = new ReadOnlyCollection<Permission>(_all.Where(p => p.IsRoot).ToArray());
    public static IReadOnlyList<Permission> Admin { get; } = new ReadOnlyCollection<Permission>(_all.Where(p => !p.IsRoot).ToArray());
    public static IReadOnlyList<Permission> Basic { get; } = new ReadOnlyCollection<Permission>(_all.Where(p => p.IsBasic).ToArray());
}

public record Permission(string Description, string PermissionAction, string Resource, bool IsBasic = false, bool IsRoot = false)
{
    public string Name => NameFor(PermissionAction, Resource);
    public static string NameFor(string PermissionAction, string resource) => $"Permissions.{resource}.{PermissionAction}";
}