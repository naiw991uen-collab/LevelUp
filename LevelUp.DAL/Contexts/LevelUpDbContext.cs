using System.Reflection;
using LevelUp.DAL.Applications;
using LevelUp.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.DAL.Contexts;

public class LevelUpDbContext : IdentityDbContext<ApplicationUser>
{
    public LevelUpDbContext(DbContextOptions<LevelUpDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HaveConversion<double>();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetAuditDates();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        SetAuditDates();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void SetAuditDates()
    {
        var now = DateTime.Now;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = now;
            else if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = now;
        }
    }

    public DbSet<Room> Rooms { get; set; }
    public DbSet<Drink> Drinks { get; set; }
    public DbSet<ModeRate> ModeRates { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
    public DbSet<SessionSegment> SessionSegments { get; set; }
    public DbSet<SessionDrink> SessionDrinks { get; set; }
}
