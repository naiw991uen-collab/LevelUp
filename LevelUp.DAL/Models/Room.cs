using LevelUp.DAL.Models.Enums;

namespace LevelUp.DAL.Models;

public class Room : BaseEntity
{
    public string Name { get; set; } = null!;
    public GameMode DefaultMode { get; set; }
    public bool IsActive { get; set; } = true;

    #region Relationships

    public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
    public ICollection<SessionDrink> SessionDrinks { get; set; } = new List<SessionDrink>();

    #endregion
}
