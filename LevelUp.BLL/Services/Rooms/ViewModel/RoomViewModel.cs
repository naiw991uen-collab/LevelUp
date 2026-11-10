using LevelUp.DAL.Models.Enums;

namespace LevelUp.BLL.Services.Rooms.ViewModel;

// One row in the rooms table
public class RoomViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public GameMode DefaultMode { get; set; }
    public bool IsActive { get; set; }
    public SessionStatus? SessionStatus { get; set; }
    public GameMode? CurrentMode { get; set; }
    public bool HasOpenSession => SessionStatus is not null;
}
