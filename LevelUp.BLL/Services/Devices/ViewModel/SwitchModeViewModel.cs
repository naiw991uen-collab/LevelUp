using LevelUp.DAL.Models.Enums;

namespace LevelUp.BLL.Services.Devices.ViewModel;

public class SwitchModeViewModel
{
    public int RoomId { get; set; }
    public GameMode Mode { get; set; }
}
