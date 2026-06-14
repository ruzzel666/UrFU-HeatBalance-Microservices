using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ProductWebApp.Data;

public static class DbSeed
{
    public const string AdminRoleName = "Admin";

    public static async Task InitializeAsync(IServiceProvider services, IConfiguration config, ILogger logger)
    {
        await using var scope = services.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        logger.LogInformation("Initializing database...");
        try
        {
            if (db.Database.IsSqlite())
            {
                // Для локального режима SQLite используем создание схемы без миграций (миграции у нас под PostgreSQL).
                await db.Database.EnsureCreatedAsync();
            }
            else
            {
                await db.Database.MigrateAsync();
            }
        }
        catch (NpgsqlException ex)
        {
            logger.LogError(ex,
                "Не удалось подключиться к PostgreSQL. Проверьте, что PostgreSQL запущен и строка подключения верна. " +
                "Приложение продолжит работу, но функции, зависящие от БД, будут недоступны.");
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Ошибка при применении миграций БД. Приложение продолжит работу, но функции, зависящие от БД, будут недоступны.");
            return;
        }

        if (!await roleManager.RoleExistsAsync(AdminRoleName))
        {
            logger.LogInformation("Creating role {Role}", AdminRoleName);
            var roleResult = await roleManager.CreateAsync(new IdentityRole(AdminRoleName));
            if (!roleResult.Succeeded)
                throw new InvalidOperationException($"Не удалось создать роль '{AdminRoleName}': {string.Join("; ", roleResult.Errors.Select(e => e.Description))}");
        }

        var adminEmail = config["Seed:AdminEmail"];
        var adminPassword = config["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning("Seed admin credentials are not configured (Seed:AdminEmail / Seed:AdminPassword).");
            return;
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            logger.LogInformation("Creating admin user {Email}", adminEmail);
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (!createResult.Succeeded)
                throw new InvalidOperationException($"Не удалось создать администратора '{adminEmail}': {string.Join("; ", createResult.Errors.Select(e => e.Description))}");
        }

        if (!await userManager.IsInRoleAsync(adminUser, AdminRoleName))
        {
            logger.LogInformation("Adding user {Email} to role {Role}", adminEmail, AdminRoleName);
            var addRoleResult = await userManager.AddToRoleAsync(adminUser, AdminRoleName);
            if (!addRoleResult.Succeeded)
                throw new InvalidOperationException($"Не удалось назначить роль '{AdminRoleName}' пользователю '{adminEmail}': {string.Join("; ", addRoleResult.Errors.Select(e => e.Description))}");
        }
    }
}

