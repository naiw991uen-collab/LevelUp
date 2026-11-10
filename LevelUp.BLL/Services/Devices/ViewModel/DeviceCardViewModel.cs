using LevelUp.DAL.Models.Enums;

namespace LevelUp.BLL.Services.Devices.ViewModel;

public class DeviceCardViewModel
{
    public int RoomId { get; set; }
    public string RoomName { get; set; } = default!;
    public GameMode DefaultMode { get; set; }

    public SessionStatus? Status { get; set; }
    public GameMode? CurrentMode { get; set; }
    public decimal CurrentRate { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? StoppedAt { get; set; }

    public IReadOnlyList<DeviceSegmentViewModel> Segments { get; set; } = [];
    public IReadOnlyList<DeviceDrinkViewModel> Drinks { get; set; } = [];

    public bool IsIdle => Status is null;
    public bool IsRunning => Status == SessionStatus.Running;
    public bool IsStopped => Status == SessionStatus.Stopped;
    public bool HasSwitched => Segments.Count > 1;

    public int ElapsedSeconds => Segments.Sum(s => s.Seconds);
    public int SegmentElapsedSeconds => Segments.FirstOrDefault(s => s.IsCurrent)?.Seconds ?? 0;
    public decimal BaseCost => Segments.Where(s => !s.IsCurrent).Sum(s => s.Cost);
    public decimal PlayCost => Segments.Sum(s => s.Cost);
    public decimal DrinksTotal => Drinks.Sum(d => d.Total);
    public decimal Total => PlayCost + DrinksTotal;
}
