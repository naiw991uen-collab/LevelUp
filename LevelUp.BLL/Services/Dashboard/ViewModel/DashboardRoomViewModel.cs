using LevelUp.BLL.Services.Devices.ViewModel;
using LevelUp.DAL.Models.Enums;

namespace LevelUp.BLL.Services.Dashboard.ViewModel;

public class DashboardRoomViewModel
{
    public string RoomName { get; set; } = default!;
    public bool IsActive { get; set; }
    public SessionStatus? Status { get; set; }
    public GameMode? CurrentMode { get; set; }

    public int Seconds { get; set; }
    public decimal PlayCost { get; set; }
    public IReadOnlyList<DeviceDrinkViewModel> Drinks { get; set; } = [];

    public decimal DrinksTotal => Drinks.Sum(d => d.Total);
    public decimal Total => PlayCost + DrinksTotal;
}
