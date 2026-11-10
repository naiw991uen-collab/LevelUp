using System.Diagnostics;
using LevelUp.DAL.Applications;
using LevelUp.DAL.Contexts;
using LevelUp.DAL.DataSeed;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.PL.Extentions;

public static class ProgramExtention
{
    public static async Task SeedDataAndApplyPendingMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<LevelUpDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Applied migrations: {Migrations}", string.Join(", ", pendingMigrations));
        }

        await ModeRateDataSeeding.SeedAsync(dbContext, logger);

        await IdentityDataSeeding.SeedIdentityDataAsync(roleManager, userManager, logger);
    }

    public static void OpenBrowserOnStart(this WebApplication app, string path = "/Account/Login")
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            var address = app.Services.GetRequiredService<IServer>()
                .Features.Get<IServerAddressesFeature>()?.Addresses.FirstOrDefault()
                ?? "http://localhost:5080";

            var url = address.TrimEnd('/') + path;

            logger.LogInformation("LEVEL UP started on {Url}", url);

            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Could not open the browser, open {Url} manually", url);
            }
        });
    }
}
