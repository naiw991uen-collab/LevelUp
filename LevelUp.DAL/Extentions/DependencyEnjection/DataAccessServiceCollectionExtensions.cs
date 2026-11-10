using LevelUp.DAL.Contexts;
using LevelUp.DAL.Repositories.Classes;
using LevelUp.DAL.Repositories.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LevelUp.DAL.Extentions.DependencyEnjection;

public static class DataAccessServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LevelUpDbContext>(options =>
        {
            options.UseSqlite(BuildConnectionString(configuration));
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Scan(scan => scan
            .FromAssemblyOf<GameSessionRepository>()
            .AddClasses(c => c.Where(t => t.Name.EndsWith("Repository")))
            .AsMatchingInterface()
            .WithScopedLifetime());

        return services;
    }

    // levelup.db always lives next to LevelUp.exe (the flash drive), whatever the current directory is
    private static string BuildConnectionString(IConfiguration configuration)
    {
        var builder = new SqliteConnectionStringBuilder(
            configuration.GetConnectionString("DefaultConnection") ?? "Data Source=levelup.db");

        if (!Path.IsPathRooted(builder.DataSource))
            builder.DataSource = Path.Combine(GetAppFolder(), builder.DataSource);

        return builder.ToString();
    }

    private static string GetAppFolder()
    {
        // Single-file publish: Assembly.Location is empty, AppContext.BaseDirectory is the exe folder.
        // dotnet ef (design time): AppContext.BaseDirectory is the tool folder, Assembly.Location is bin.
        var assemblyLocation = typeof(LevelUpDbContext).Assembly.Location;

        return string.IsNullOrEmpty(assemblyLocation)
            ? AppContext.BaseDirectory
            : Path.GetDirectoryName(assemblyLocation)!;
    }
}
