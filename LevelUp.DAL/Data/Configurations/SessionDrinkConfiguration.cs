using LevelUp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LevelUp.DAL.Data.Configurations;

public class SessionDrinkConfiguration : IEntityTypeConfiguration<SessionDrink>
{
    public void Configure(EntityTypeBuilder<SessionDrink> builder)
    {
        builder.HasOne(X => X.GameSession)
               .WithMany(X => X.SessionDrinks)
               .HasForeignKey(X => X.GameSessionId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(X => X.Room)
               .WithMany(X => X.SessionDrinks)
               .HasForeignKey(X => X.RoomId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(X => X.Drink)
               .WithMany(X => X.SessionDrinks)
               .HasForeignKey(X => X.DrinkId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(X => new { X.RoomId, X.GameSessionId });

        builder.ToTable(Tb =>
        {
            Tb.HasCheckConstraint("SessionDrinkQuantityCheck",
                "Quantity > 0");

            Tb.HasCheckConstraint("SessionDrinkUnitPriceCheck",
                "UnitPrice > 0");
        });
    }
}
