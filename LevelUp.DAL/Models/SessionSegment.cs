using LevelUp.DAL.Models.Enums;

namespace LevelUp.DAL.Models;

public class SessionSegment : BaseEntity
{
    public int GameSessionId { get; set; }
    public GameMode Mode { get; set; }
    public decimal HourlyRate { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    #region Relationships

    public GameSession GameSession { get; set; } = null!;

    #endregion
}
