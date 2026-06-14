using HeatBalance.Contracts;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace HeatBalance.Auth;

public static class OpenIddictSeed
{
    public static async Task InitializeAsync(IServiceProvider services, IConfiguration config, ILogger logger)
    {
        await using var scope = services.CreateAsyncScope();
        var provider = scope.ServiceProvider;

        var db = provider.GetRequiredService<AuthDbContext>();
        await db.Database.EnsureCreatedAsync();

        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roleManager.RoleExistsAsync(AuthConstants.AdminRole))
            await roleManager.CreateAsync(new IdentityRole(AuthConstants.AdminRole));

        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        var adminEmail = config["Seed:AdminEmail"] ?? "admin@example.com";
        var adminPassword = config["Seed:AdminPassword"] ?? "Admin123$";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                DisplayName = "Administrator"
            };
            await userManager.CreateAsync(adminUser, adminPassword);
        }

        if (!await userManager.IsInRoleAsync(adminUser, AuthConstants.AdminRole))
            await userManager.AddToRoleAsync(adminUser, AuthConstants.AdminRole);

        var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();
        var scopeManager = provider.GetRequiredService<IOpenIddictScopeManager>();

        foreach (var apiScope in AuthConstants.ApiScopes)
        {
            if (await scopeManager.FindByNameAsync(apiScope) is null)
            {
                await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    Name = apiScope,
                    DisplayName = apiScope,
                    Resources = { AuthConstants.ApiAudience }
                });
            }
        }

        await EnsureClientAsync(manager, new OpenIddictApplicationDescriptor
        {
            ClientId = AuthConstants.WebClientId,
            ClientType = ClientTypes.Public,
            ConsentType = ConsentTypes.Implicit,
            DisplayName = "HeatBalance Web BFF",
            RedirectUris =
            {
                new Uri("http://localhost:5262/signin-oidc"),
                new Uri("http://127.0.0.1:5262/signin-oidc")
            },
            PostLogoutRedirectUris =
            {
                new Uri("http://localhost:5262/signout-callback-oidc"),
                new Uri("http://127.0.0.1:5262/signout-callback-oidc")
            },
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles
            }
        }, AuthConstants.WebScopes);

        await EnsureClientAsync(manager, new OpenIddictApplicationDescriptor
        {
            ClientId = AuthConstants.RunServiceClientId,
            ClientSecret = AuthConstants.RunServiceClientSecret,
            ClientType = ClientTypes.Confidential,
            DisplayName = "HeatBalance Run Service",
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.ClientCredentials
            }
        }, AuthConstants.RunServiceScopes);

        logger.LogInformation("OpenIddict seed completed.");
    }

    private static async Task EnsureClientAsync(
        IOpenIddictApplicationManager manager,
        OpenIddictApplicationDescriptor descriptor,
        IEnumerable<string> scopes)
    {
        if (await manager.FindByClientIdAsync(descriptor.ClientId!) is not null)
            return;

        foreach (var scope in scopes)
            descriptor.Permissions.Add(Permissions.Prefixes.Scope + scope);

        await manager.CreateAsync(descriptor);
    }
}
