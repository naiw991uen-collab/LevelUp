using LevelUp.DAL.Models.Enums;

namespace LevelUp.DAL.Models;

public class GameSession : BaseEntity
{
    public int RoomId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public GameMode StartMode { get; set; }
    public SessionStatus Status { get; set; } = SessionStatus.Running;
    public DateOnly BusinessDay { get; set; }

    #region Relationships

    public Room Room { get; set; } = null!;
    public ICollection<SessionSegment> Segments { get; set; } = new List<SessionSegment>();
    public ICollection<SessionDrink> SessionDrinks { get; set; } = new List<SessionDrink>();

    #endregion
}
