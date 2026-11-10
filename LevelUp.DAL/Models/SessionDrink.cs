namespace LevelUp.DAL.Models;

public class SessionDrink : BaseEntity
{
    public int? GameSessionId { get; set; }
    public int RoomId { get; set; }
    public int DrinkId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    #region Relationships

    public GameSession? GameSession { get; set; }
    public Room Room { get; set; } = null!;
    public Drink Drink { get; set; } = null!;

    #endregion
}
