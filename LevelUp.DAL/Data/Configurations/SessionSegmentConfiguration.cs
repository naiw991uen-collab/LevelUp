using LevelUp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevelUp.DAL.Data.Configurations;

public class SessionSegmentConfiguration : IEntityTypeConfiguration<SessionSegment>
{
    public void Configure(EntityTypeBuilder<SessionSegment> builder)
    {
        builder.HasOne(X => X.GameSession)
               .WithMany(X => X.Segments)
               .HasForeignKey(X => X.GameSessionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(X => X.GameSessionId, "IX_SessionSegments_GameSessionId");

        // One open segment (EndedAt is NULL) per session
        builder.HasIndex(X => X.GameSessionId, "IX_SessionSegments_GameSessionId_Open")
               .IsUnique()
               .HasFilter("EndedAt IS NULL");

        builder.ToTable(Tb =>
        {
            Tb.HasCheckConstraint("SessionSegmentModeCheck",
                "Mode IN (1, 2)");

            Tb.HasCheckConstraint("SessionSegmentHourlyRateCheck",
                "HourlyRate > 0");
        });
    }
}
