namespace LevelUp.BLL.Services.Rooms.ViewModel;

public class RoomIndexViewModel
{
    public IReadOnlyList<RoomViewModel> Rooms { get; set; } = [];
    public int? EditId { get; set; }
    public CreateRoomViewModel NewRoom { get; set; } = new();

    public int TotalCount => Rooms.Count;
    public int ActiveCount => Rooms.Count(r => r.IsActive);
    public int DisabledCount => TotalCount - ActiveCount;
}
