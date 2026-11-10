using LevelUp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevelUp.DAL.Data.Configurations;

public class DrinkConfiguration : IEntityTypeConfiguration<Drink>
{
    public void Configure(EntityTypeBuilder<Drink> builder)
    {
        builder.Property(X => X.Name)
               .IsRequired()
               .HasMaxLength(50)
               .UseCollation("NOCASE");

        builder.HasIndex(X => X.Name)
               .IsUnique();

        builder.Property(X => X.IsAvailable)
               .HasDefaultValue(true);

        builder.ToTable(Tb =>
        {
            Tb.HasCheckConstraint("DrinkPriceCheck",
                "Price > 0");
        });
    }
}
