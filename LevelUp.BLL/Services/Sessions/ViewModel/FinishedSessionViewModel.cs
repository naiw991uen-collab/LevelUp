using LevelUp.BLL.Services.Devices.ViewModel;
using LevelUp.DAL.Models.Enums;

namespace LevelUp.BLL.Services.Sessions.ViewModel;

public class FinishedSessionViewModel
{
    public string RoomName { get; set; } = default!;
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    public IReadOnlyList<DeviceSegmentViewModel> Parts { get; set; } = [];
    public IReadOnlyList<DeviceDrinkViewModel> Drinks { get; set; } = [];

    public IReadOnlyList<GameMode> Modes => Parts.Select(p => p.Mode).Distinct().ToList();

    public int PlayedSeconds => Parts.Sum(p => p.Seconds);
    public decimal PlayCost => Parts.Sum(p => p.Cost);
    public decimal DrinksTotal => Drinks.Sum(d => d.Total);
    public decimal Total => PlayCost + DrinksTotal;
}
