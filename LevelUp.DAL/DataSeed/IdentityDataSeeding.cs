using LevelUp.DAL.Applications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LevelUp.DAL.DataSeed;

public static class IdentityDataSeeding
{
    private const string AdminRole = "Admin";

    private const string AdminUserName = "admin";

    public static async Task SeedIdentityDataAsync(RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager, string? adminPassword, ILogger logger)
    {
        try
        {
            if (!await roleManager.RoleExistsAsync(AdminRole))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(AdminRole));

                if (roleResult.Succeeded)
                    logger.LogInformation("Role {Role} created successfully", AdminRole);
                else
                    logger.LogError("Failed to create role {Role}: {Errors}", AdminRole,
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            if (await userManager.Users.AnyAsync())
            {
                logger.LogInformation("Identity seeding skipped: users already exist");
                return;
            }

            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                logger.LogWarning("Admin user not created: set AdminSeed:Password in appsettings.Local.json");
                return;
            }

            var admin = new ApplicationUser { UserName = AdminUserName };

            var userResult = await userManager.CreateAsync(admin, adminPassword);

            if (!userResult.Succeeded)
            {
                logger.LogError("Failed to create user {UserName}: {Errors}", admin.UserName,
                    string.Join(", ", userResult.Errors.Select(e => e.Description)));
                return;
            }

            logger.LogInformation("User {UserName} created successfully", admin.UserName);

            var addToRoleResult = await userManager.AddToRoleAsync(admin, AdminRole);

            if (addToRoleResult.Succeeded)
                logger.LogInformation("User {UserName} added to role {Role}", admin.UserName, AdminRole);
            else
                logger.LogError("Failed to add user {UserName} to role {Role}: {Errors}", admin.UserName, AdminRole,
                    string.Join(", ", addToRoleResult.Errors.Select(e => e.Description)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while seeding identity data");
        }
    }
}
