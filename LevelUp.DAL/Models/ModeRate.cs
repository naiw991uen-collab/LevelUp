using LevelUp.DAL.Models.Enums;

namespace LevelUp.DAL.Models;

public class ModeRate : BaseEntity
{
    public GameMode Mode { get; set; }
    public decimal HourlyRate { get; set; }
    public DateTime EffectiveFrom { get; set; }
}
