using LevelUp.DAL.Models.Enums;

namespace LevelUp.BLL.Services.Rooms.ViewModel;

public class UpdateRoomDefaultModeViewModel
{
    public int Id { get; set; }
    public GameMode DefaultMode { get; set; }
}
