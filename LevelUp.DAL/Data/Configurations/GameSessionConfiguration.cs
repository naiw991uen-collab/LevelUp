using LevelUp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevelUp.DAL.Data.Configurations;

public class GameSessionConfiguration : IEntityTypeConfiguration<GameSession>
{
    public void Configure(EntityTypeBuilder<GameSession> builder)
    {
        builder.HasOne(X => X.Room)
               .WithMany(X => X.GameSessions)
               .HasForeignKey(X => X.RoomId)
               .OnDelete(DeleteBehavior.Restrict);

        // One open (Running / Stopped) session per room
        builder.HasIndex(X => X.RoomId)
               .IsUnique()
               .HasFilter("Status IN (1, 2)")
               .HasDatabaseName("IX_GameSessions_RoomId_Open");

        builder.HasIndex(X => X.BusinessDay);

        builder.ToTable(Tb =>
        {
            Tb.HasCheckConstraint("GameSessionStartModeCheck",
                "StartMode IN (1, 2)");

            Tb.HasCheckConstraint("GameSessionStatusCheck",
                "Status IN (1, 2, 3)");
        });
    }
}
