using LevelUp.DAL.Models.Enums;

namespace LevelUp.BLL.Services.Rooms.ViewModel;

public class CreateRoomViewModel
{
    public string Name { get; set; } = default!;
    public GameMode DefaultMode { get; set; } = GameMode.Single;
}
