namespace LevelUp.DAL.Models;

public class Drink : BaseEntity
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;

    #region Relationships

    public ICollection<SessionDrink> SessionDrinks { get; set; } = new List<SessionDrink>();

    #endregion
}
