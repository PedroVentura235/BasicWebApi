using Application.Identity;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using BasicWebApi.Infrastructure.Auth;
using Infrastructure.Auth.Permissions;
using Infrastructure.Common;
using Infrastructure.Exceptions;
using Infrastructure.Identity;
using Infrastructure.Identity.KeyCloak;
using Infrastructure.Identity.Services;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddEndpointsApiExplorer();
        services.AddApiVersioning();
        services.AddPersistence();

        services.AddHttpClient();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IKeyCloakService, KeyCloakService>();

        services.AddTransient<ICurrentUser, CurrentUser>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddPermissions();
        services.AddCustomIdentity();
        services.AddKeycloakAuthorization(config);

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddServices();

        services.AddCurrentUser();

        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseCurrentUser();
        return app;
    }

    private static IServiceCollection AddApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"));
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    private static IServiceCollection AddPermissions(this IServiceCollection services) =>
        services
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

    internal static IApplicationBuilder UseCurrentUser(this WebApplication app) =>
        app.UseMiddleware<CurrentUserMiddleware>();

    private static IServiceCollection AddCurrentUser(this IServiceCollection services) =>
        services
            .AddScoped<CurrentUserMiddleware>()
            .AddScoped<ICurrentUser, CurrentUser>()
            .AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());

    public static async Task InitializeDatabasesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        using var scope = services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>()
            .InitializeDatabasesAsync(cancellationToken);
    }
}