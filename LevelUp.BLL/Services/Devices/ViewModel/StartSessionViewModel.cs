using LevelUp.DAL.Models.Enums;

namespace LevelUp.BLL.Services.Devices.ViewModel;

public class StartSessionViewModel
{
    public int RoomId { get; set; }
    public GameMode Mode { get; set; }
}
