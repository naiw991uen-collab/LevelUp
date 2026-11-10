using LevelUp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevelUp.DAL.Data.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.Property(X => X.Name)
               .IsRequired()
               .HasMaxLength(50)
               .UseCollation("NOCASE");

        builder.HasIndex(X => X.Name)
               .IsUnique();

        builder.Property(X => X.IsActive)
               .HasDefaultValue(true);

        builder.ToTable(Tb =>
        {
            Tb.HasCheckConstraint("RoomDefaultModeCheck",
                "DefaultMode IN (1, 2)");
        });
    }
}
