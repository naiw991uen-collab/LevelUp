using LevelUp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevelUp.DAL.Data.Configurations;

public class ModeRateConfiguration : IEntityTypeConfiguration<ModeRate>
{
    public void Configure(EntityTypeBuilder<ModeRate> builder)
    {
        builder.HasIndex(X => new { X.Mode, X.EffectiveFrom });

        builder.ToTable(Tb =>
        {
            Tb.HasCheckConstraint("ModeRateModeCheck",
                "Mode IN (1, 2)");

            Tb.HasCheckConstraint("ModeRateHourlyRateCheck",
                "HourlyRate > 0");
        });
    }
}
